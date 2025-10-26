using UnityEngine;

public class PlayerController : MonoBehaviour
{
    LayerMask layerMask;
    public static GameObject Grabbable;
    public Transform Grabarea;
    public Transform Item_find;
    private CharacterController _characterController;
    [Header("Movement")]
    public float MovementSpeed = 10f;
    public float RotationSpeed = 5f;      // W 전진 시 카메라 정면으로 맞추는 속도
    public float JumpForce = 10f;
    public float Gravity = -30f;

    [Header("Camera")]
    [SerializeField] private Transform camTransform; // 비우면 Camera.main 자동 할당

    [Header("Mouse Look (안전 장치 포함)")]
    public float MouseSensitivity = 0.1f;     // 감도(작게 시작)
    public float MaxYawPerFrameDeg = 2.0f;    // 1프레임당 적용 가능한 최대 각도(도)
    public float SpikeCutoff = 50f;           // 이 값 이상 델타(픽셀)는 스파이크로 간주하고 무시
    public bool RequireLockedCursor = true;   // 잠금 상태일 때만 회전 입력 처리

    public float YawSmoothTime = 0.06f;       // 회전 스무딩

    // 내부 회전 상태
    private float _targetYaw;     // 목표 Yaw(마우스/자동정렬이 갱신)
    private float _currentYaw;    // 실제 적용 Yaw(스무딩 결과)
    private float _yawVel;        // SmoothDampAngle 속도

    private bool _wantAlignThisFrame; // 이번 프레임 전진 정렬 요청 플래그
    private float _verticalVelocity;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Item", "Player");
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        if (camTransform == null && Camera.main != null)
            camTransform = Camera.main.transform;

        _currentYaw = _targetYaw = transform.eulerAngles.y;
    }

    // 이동: 회전은 '요청'만, 실제 적용은 LateUpdate에서만
    public void Move(Vector2 movementVector)
    {
        // 카메라 기준 이동 축
        Vector3 fwd, right;
        if (camTransform != null)
        {
            fwd = camTransform.forward; fwd.y = 0f; if (fwd.sqrMagnitude > 0.0001f) fwd.Normalize();
            right = camTransform.right; right.y = 0f; if (right.sqrMagnitude > 0.0001f) right.Normalize();
        }
        else
        {
            fwd = transform.forward; right = transform.right;
        }

        Vector3 moveDir = fwd * movementVector.y + right * movementVector.x;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // 중력
        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < 0f) _verticalVelocity = -0.5f;
        }
        else
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }

        // 전진 입력 시 카메라 Yaw로 정렬 "요청"
        _wantAlignThisFrame = (movementVector.y > 0.05f && camTransform != null);

        // 이동 적용
        Vector3 velocity = moveDir * MovementSpeed;
        velocity.y = _verticalVelocity;
        _characterController.Move(velocity * Time.deltaTime);
    }

    // 마우스 좌우 입력 -> 목표각만 갱신 (transform.rotation 직접 만지지 않음)
    public void Rotate(Vector2 rotationVector)
    {
        // 잠금 상태 요구 시, 잠금 아닐 땐 무시
        if (RequireLockedCursor && Cursor.lockState != CursorLockMode.Locked) return;

        // 스파이크 컷(게임 뷰 포커스 이동/해제 등으로 튀는 델타 무시)
        float dx = rotationVector.x;
        if (Mathf.Abs(dx) > SpikeCutoff) return;

        // 1프레임 최대 반영 각도 제한
        float deltaDeg = Mathf.Clamp(dx * MouseSensitivity, -MaxYawPerFrameDeg, +MaxYawPerFrameDeg);

        _targetYaw = Mathf.Repeat(_targetYaw + deltaDeg, 360f);
    }

    public void Jump()
    {
        if (_characterController.isGrounded)
            _verticalVelocity = JumpForce;
    }

    public void Grab()
    {
        if (Grabbable != null) return;
        Debug.DrawRay(Item_find.position, camTransform.forward, Color.red, 5f);
        if (Physics.Raycast(Item_find.position, camTransform.forward, out RaycastHit hit, 5.0f, layerMask))
        {
            Grabbable = hit.transform.gameObject;
            Grabbable.transform.SetParent(Grabarea, worldPositionStays: true);
        }
        
        //OPTION 2 - you could also try implementing 3 raycasts that each shoot forward
        //cast one from itemhold, one above itemhohld, and one below itemhold
        //I do not recommend this option, as it will not be as accurate as option 1, 
        //but you could certainly go this route!
    }

    public void Drop()
    {
        if (Grabbable == null) return;
        Grabbable.transform.SetParent(null, worldPositionStays: true);
        Grabbable = null;
    }

    public bool HasEquipped()
    {
        return Grabbable != null && Grabbable.name == "Mop";
    }

    // 회전 최종 적용은 오직 여기에서만
    void LateUpdate()
    {
        // 전진 정렬 요청이 있으면 목표각을 카메라 Yaw로 교체(즉시 반영 X, 스무딩으로 반영)
        if (_wantAlignThisFrame && camTransform != null)
        {
            _targetYaw = camTransform.eulerAngles.y;
            _wantAlignThisFrame = false;
        }

        // 목표각 -> 현재각 스무딩 후 최종 적용
        _currentYaw = Mathf.SmoothDampAngle(_currentYaw, _targetYaw, ref _yawVel, YawSmoothTime);
        transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);
    }

    // 포커스 복귀 시 커서 잠그기(원하면 유지)
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && RequireLockedCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
