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
        {
            Debug.Log("POS ZONE ENTER");
            if (switcher) switcher.SetPOS();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("POS ZONE EXIT");
            if (switcher) switcher.SetTPS();
        } 
    }
}
