using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemView : MonoBehaviour
{
    [SerializeField] private ItemSO _item;

    private PosManagerUI _pos;
    private NpcController _npc;

    /// <summary>NPC가 스폰 후 즉시 호출</summary>
    public void Bind(ItemSO item, PosManagerUI pos, NpcController owner)
    {
        _item = item;
        _pos = pos;
        _npc = owner;
    }

    // 3D 오브젝트 클릭 (Collider 필요)
    private void OnMouseDown()
    {
        if (_item == null || _pos == null || _npc == null) return;

        _pos.AddItemLine(_item); // POS 장바구니에 추가
        _npc.OneItemScanned();   // NPC에 "한 개 처리됨" 알림
        Destroy(gameObject);     // 클릭된 물건 제거
    }
}
