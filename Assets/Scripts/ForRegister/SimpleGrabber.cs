using UnityEngine;

public class SimpleGrabber : MonoBehaviour
{
    public Camera cam;
    public Transform handAnchor; // 카메라 앞 자식 등

    private CardToken holding;

    void Awake() { if (!cam) cam = Camera.main; }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (holding == null) TryPick();
            else TryDrop();
        }
    }

    void TryPick()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            var card = hit.collider.GetComponentInParent<CardToken>();
            if (card != null) { holding = card; card.PickUp(handAnchor); }
        }
    }

    void TryDrop()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            holding.Drop(hit.point);
            holding = null;
        }
    }
}
