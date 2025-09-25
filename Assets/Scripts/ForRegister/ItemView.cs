using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemView : MonoBehaviour
{
    [SerializeField] private ItemSO _item;

    private PosManagerUI _pos;
    private NpcController _npc;

    /// <summary>NPC�� ���� �� ��� ȣ��</summary>
    public void Bind(ItemSO item, PosManagerUI pos, NpcController owner)
    {
        _item = item;
        _pos = pos;
        _npc = owner;
    }

    // 3D ������Ʈ Ŭ�� (Collider �ʿ�)
    private void OnMouseDown()
    {
        if (_item == null || _pos == null || _npc == null) return;

        _pos.AddItemLine(_item); // POS ��ٱ��Ͽ� �߰�
        _npc.OneItemScanned();   // NPC�� "�� �� ó����" �˸�
        Destroy(gameObject);     // Ŭ���� ���� ����
    }
}
