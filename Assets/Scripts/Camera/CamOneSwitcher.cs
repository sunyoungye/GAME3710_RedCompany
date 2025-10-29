using System.Collections;
using UnityEngine;

public class CamOneSwitcher : MonoBehaviour
{
    [Header("Single Physical Camera")]
    public Camera mainCam;

    [Header("Mounts")]
    public Transform tpsMount; // 일반 플레이 위치/각도
    public Transform posMount; // POS 위치/각도

    [Header("Blend")]
    public float blendTime = 0.25f;

    Coroutine _co;

    public void ToPOS() => SwitchTo(posMount);
    public void ToTPS() => SwitchTo(tpsMount);

    void SwitchTo(Transform target)
    {
        if (!mainCam || !target) return;
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(BlendTo(target));
    }

    IEnumerator BlendTo(Transform target)
    {
        var t0 = 0f;
        var startPos = mainCam.transform.position;
        var startRot = mainCam.transform.rotation;

        while (t0 < blendTime)
        {
            t0 += Time.deltaTime;
            float a = Mathf.SmoothStep(0f, 1f, t0 / blendTime);
            mainCam.transform.position = Vector3.Lerp(startPos, target.position, a);
            mainCam.transform.rotation = Quaternion.Slerp(startRot, target.rotation, a);
            yield return null;
        }

        mainCam.transform.SetPositionAndRotation(target.position, target.rotation);
    }

    // 옵션: 시작 시 TPS로 스냅
    void Start()
    {
        if (tpsMount && mainCam)
            mainCam.transform.SetPositionAndRotation(tpsMount.position, tpsMount.rotation);
    }
}
