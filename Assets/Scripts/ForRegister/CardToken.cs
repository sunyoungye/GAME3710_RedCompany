using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardToken : MonoBehaviour
{
    [HideInInspector] public NpcController owner;
    public bool isHeld { get; private set; }
    private Transform _holdAnchor;

    public void Init(NpcController who) { owner = who; }

    // SimpleGrabber 호환(인자 없는 버전)
    public void PickUp()
    {
        isHeld = true;
        var col = GetComponent<Collider>(); if (col) col.enabled = false;
        if (_holdAnchor)
        {
            transform.SetParent(_holdAnchor, worldPositionStays: false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
    public void PickUp(Transform anchor) { _holdAnchor = anchor; PickUp(); }

    public void Drop() { isHeld = false; transform.SetParent(null); var col = GetComponent<Collider>(); if (col) col.enabled = true; }
    public void Drop(Vector3 worldPos) { Drop(); transform.position = worldPos; }
}
