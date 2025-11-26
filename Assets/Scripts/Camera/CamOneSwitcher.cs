using UnityEngine;

public class CamOneSwitcher : MonoBehaviour
{
    public Camera tpsCam;   // 처음에 보이는 TPS 카메라
    public Camera posCam;   // POS에서 보이는 카메라

    void Start()
    {
        SetTPS();   // 시작할 때는 TPS 카메라만 켜두기
    }

    public void SetTPS()
    {
        if (tpsCam) tpsCam.enabled = true;
        if (posCam) posCam.enabled = false;
        Debug.Log("[CamOneSwitcher] TPS cam ON");
    }

    public void SetPOS()
    {
        if (tpsCam) tpsCam.enabled = false;
        if (posCam) posCam.enabled = true;
        Debug.Log("[CamOneSwitcher] POS cam ON");
    }
}
