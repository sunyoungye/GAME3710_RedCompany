using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardToken : MonoBehaviour
{
    [HideInInspector] public NpcController owner;

    // ✅ 추가: 이 카드를 관리하는 정확한 POS 인스턴스
    public PosManagerUI pos { get; private set; }

    public bool isHeld { get; private set; }
    private Transform _holdAnchor;

    // ✅ 수정: POS도 함께 주입
    public void Init(NpcController who, PosManagerUI posRef)
    {
        owner = who;
        pos = posRef;
    }

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
