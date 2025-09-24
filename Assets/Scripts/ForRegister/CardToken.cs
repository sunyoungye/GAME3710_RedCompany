using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardToken : MonoBehaviour
{
    [HideInInspector] public NpcController owner;
    public bool isHeld { get; private set; }
    private Transform _holdAnchor;

    public void Init(NpcController who) { owner = who; }

    public void PickUp(Transform anchor)
    {
        isHeld = true; _holdAnchor = anchor;
        var col = GetComponent<Collider>(); if (col) col.enabled = false;
        transform.SetParent(anchor, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Vector3 worldPos)
    {
        isHeld = false; _holdAnchor = null;
        transform.SetParent(null);
        transform.position = worldPos;
        var col = GetComponent<Collider>(); if (col) col.enabled = true;
    }
}
