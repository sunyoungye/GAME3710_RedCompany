using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPerson : MonoBehaviour
{
    [Header("Target")]
    public Transform player;              // 플레이어 루트(필수)

    [Header("Framing (closer & higher)")]
    public float height = 2.0f;           // 카메라 높이 (더 위로 보고 싶으면 ↑)
    public float distance = 2.3f;         // 플레이어 뒤 거리 (가까이 = 2.0~2.5)
    public float lateral = 0.0f;          // 좌우 오프셋(어깨 카메라 느낌 주고 싶으면 0.3)

    [Header("Feel")]
    public float positionSmoothTime = 0.08f;  // 위치 부드러움
    public float rotationLerp = 18f;          // 회전 반응 속도

    [Header("Look Up/Down (only mouse Y)")]
    public float pitch = 8f;              // 초기 상하각(조금만 올려둠)
    public float minPitch = -10f;
    public float maxPitch = 45f;
    public float pitchSensitivity = 28f;  // 느리게(체감 천천히)
    public float pitchSmoothTime = 0.28f; // 부드럽게 딜레이

    [Header("Options")]
    public bool lockRoll = true;

    // internal
    Vector3 vel;
    float currentPitch, pitchVel;

    void LateUpdate()
    {
        if (!player) return;

        // --- 마우스 Y로만 천천히 pitch 조절 ---
        var mouse = Mouse.current;
        if (mouse != null)
        {
            float dy = mouse.delta.ReadValue().y;
            pitch -= dy * pitchSensitivity * Time.deltaTime;  // 위로 올리면 위로 보이게
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        currentPitch = Mathf.SmoothDampAngle(currentPitch, pitch, ref pitchVel, pitchSmoothTime);

        // --- 플레이어 뒤/위의 기본 오프셋(진짜 기본 3인칭) ---
        // local offset: (lateral, height, -distance), 여기에 "위/아래 보기"만 살짝 적용
        Vector3 localOffset = new Vector3(lateral, height, -distance);
        Quaternion pitchLocal = Quaternion.Euler(currentPitch, 0f, 0f);
        Vector3 pitchedLocalOffset = pitchLocal * localOffset;

        // 월드 위치로 변환 (플레이어 yaw를 자동 추종)
        Vector3 desiredPos = player.TransformPoint(pitchedLocalOffset);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref vel, positionSmoothTime);

        // 바라보는 지점: 플레이어 상체 쪽을 자연스럽게
        Vector3 lookTarget = player.position + Vector3.up * (height * 0.8f);
        Quaternion targetRot = Quaternion.LookRotation((lookTarget - transform.position).normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationLerp * Time.deltaTime);

        if (lockRoll)
        {
            var e = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(e.x, e.y, 0f);
        }
    }
}
