using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PosCameraZone : MonoBehaviour
{
    public CamOneSwitcher switcher;
    public string playerTag = "Player";

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            switcher.mode = CamOneSwitcher.Mode.POS;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            switcher.mode = CamOneSwitcher.Mode.TPS;
    }
}
