using UnityEngine;

public class CamOneSwitcher : MonoBehaviour
{
    public enum Mode { TPS, POS }
    public Mode mode = Mode.TPS;

    [Header("Mounts")]
    public Transform tpsMount;   
    public Transform posMount;  

    [Header("Smoothing")]
    public float moveSpeed = 6f;   
    public float rotSpeed = 10f;  

    Transform Target =>
        mode == Mode.POS ? posMount : tpsMount;

    void LateUpdate()
    {
        if (!Target) return;

        float tPos = 1f - Mathf.Exp(-moveSpeed * Time.deltaTime);
        float tRot = 1f - Mathf.Exp(-rotSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, Target.position, tPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, Target.rotation, tRot);
    }
}
