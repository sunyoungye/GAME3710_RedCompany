using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Raycast Layers")]
    [SerializeField] private LayerMask layerMask; // Set in Inspector: include "Card", "Item", etc.

    public static GameObject Grabbable;
    public Transform Grabarea;
    public Transform Item_find;

    private CharacterController _characterController;

    [Header("Movement")]
    public float MovementSpeed = 10f;
    public float RotationSpeed = 5f;
    public float JumpForce = 10f;
    public float Gravity = -30f;

    [Header("Camera")]
    [SerializeField] private Transform camTransform; // Will be set by CameraSwitcher via SetCam()

    [Header("Mouse Look")]
    public float MouseSensitivity = 0.1f;
    public float MaxYawPerFrameDeg = 2.0f;
    public float SpikeCutoff = 50f;
    public bool RequireLockedCursor = true;
    public float YawSmoothTime = 0.06f;

    // Lock only movement/jump; keep interaction alive in POS
    public bool ControlsLocked = false;

    private float _targetYaw;
    private float _currentYaw;
    private float _yawVel;

    private bool _wantAlignThisFrame;
    private float _verticalVelocity;

    void Awake()
    {
        /*layerMask = LayerMask.GetMask("Item", "Player");*/
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        if (camTransform == null && Camera.main != null)
            camTransform = Camera.main.transform;

        _currentYaw = _targetYaw = transform.eulerAngles.y;
    }

    public void SetCam(Transform t)
    {
        camTransform = t;
    }

    // Movement: guard by ControlsLocked
    public void Move(Vector2 movementVector)
    {
        if (ControlsLocked) return;

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

        // �߷�
        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < 0f) _verticalVelocity = -0.5f;
        }
        else
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }

        // ���� �Է� �� ī�޶� Yaw�� ���� "��û"
        _wantAlignThisFrame = (movementVector.y > 0.05f && camTransform != null);

        // �̵� ����
        Vector3 velocity = moveDir * MovementSpeed;
        velocity.y = _verticalVelocity;
        _characterController.Move(velocity * Time.deltaTime);
    }

    // ���콺 �¿� �Է� -> ��ǥ���� ���� (transform.rotation ���� ������ ����)
    public void Rotate(Vector2 rotationVector)
    {
        // ��� ���� �䱸 ��, ��� �ƴ� �� ����
        if (RequireLockedCursor && Cursor.lockState != CursorLockMode.Locked) return;

        // ������ũ ��(���� �� ��Ŀ�� �̵�/���� ������ Ƣ�� ��Ÿ ����)
        float dx = rotationVector.x;
        if (Mathf.Abs(dx) > SpikeCutoff) return;

        // 1������ �ִ� �ݿ� ���� ����
        float deltaDeg = Mathf.Clamp(dx * MouseSensitivity, -MaxYawPerFrameDeg, +MaxYawPerFrameDeg);

        _targetYaw = Mathf.Repeat(_targetYaw + deltaDeg, 360f);
    }

    public void Jump()
    {
        if (ControlsLocked) return;
        if (_characterController.isGrounded)
            _verticalVelocity = JumpForce;
    }

    public void Grab()
    {
        if (Grabbable != null) return;
        if (camTransform == null) return;

        Debug.DrawRay(Item_find.position, camTransform.forward * 5f, Color.red, 0.25f);

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

    // ȸ�� ���� ������ ���� ���⿡����
    void LateUpdate()
    {
        // ���� ���� ��û�� ������ ��ǥ���� ī�޶� Yaw�� ��ü(��� �ݿ� X, ���������� �ݿ�)
        if (_wantAlignThisFrame && camTransform != null)
        {
            _targetYaw = camTransform.eulerAngles.y;
            _wantAlignThisFrame = false;
        }

        // ��ǥ�� -> ���簢 ������ �� ���� ����
        _currentYaw = Mathf.SmoothDampAngle(_currentYaw, _targetYaw, ref _yawVel, YawSmoothTime);
        transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && RequireLockedCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
