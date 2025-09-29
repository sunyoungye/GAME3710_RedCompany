using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPerson : MonoBehaviour
{
    [Header("Target")]
    public Transform player;              // �÷��̾� ��Ʈ(�ʼ�)

    [Header("Framing (closer & higher)")]
    public float height = 2.0f;           // ī�޶� ���� (�� ���� ���� ������ ��)
    public float distance = 2.3f;         // �÷��̾� �� �Ÿ� (������ = 2.0~2.5)
    public float lateral = 0.0f;          // �¿� ������(��� ī�޶� ���� �ְ� ������ 0.3)

    [Header("Feel")]
    public float positionSmoothTime = 0.08f;  // ��ġ �ε巯��
    public float rotationLerp = 18f;          // ȸ�� ���� �ӵ�

    [Header("Look Up/Down (only mouse Y)")]
    public float pitch = 8f;              // �ʱ� ���ϰ�(���ݸ� �÷���)
    public float minPitch = -10f;
    public float maxPitch = 45f;
    public float pitchSensitivity = 28f;  // ������(ü�� õõ��)
    public float pitchSmoothTime = 0.28f; // �ε巴�� ������

    [Header("Options")]
    public bool lockRoll = true;

    // internal
    Vector3 vel;
    float currentPitch, pitchVel;

    void LateUpdate()
    {
        if (!player) return;

        // --- ���콺 Y�θ� õõ�� pitch ���� ---
        var mouse = Mouse.current;
        if (mouse != null)
        {
            float dy = mouse.delta.ReadValue().y;
            pitch -= dy * pitchSensitivity * Time.deltaTime;  // ���� �ø��� ���� ���̰�
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        currentPitch = Mathf.SmoothDampAngle(currentPitch, pitch, ref pitchVel, pitchSmoothTime);

        // --- �÷��̾� ��/���� �⺻ ������(��¥ �⺻ 3��Ī) ---
        // local offset: (lateral, height, -distance), ���⿡ "��/�Ʒ� ����"�� ��¦ ����
        Vector3 localOffset = new Vector3(lateral, height, -distance);
        Quaternion pitchLocal = Quaternion.Euler(currentPitch, 0f, 0f);
        Vector3 pitchedLocalOffset = pitchLocal * localOffset;

        // ���� ��ġ�� ��ȯ (�÷��̾� yaw�� �ڵ� ����)
        Vector3 desiredPos = player.TransformPoint(pitchedLocalOffset);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref vel, positionSmoothTime);

        // �ٶ󺸴� ����: �÷��̾� ��ü ���� �ڿ�������
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
