using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemView : MonoBehaviour
{
    public ItemSO item;
    private PosManagerUI _pos;

    [SerializeField] bool autoScanOnBind = true;

    public void Bind(ItemSO so, PosManagerUI pos)
    {
        item = so;
        _pos = pos;
        Debug.Log($"[ItemView.Bind] {item?.displayName}  pos={(pos ? pos.name : "NULL")}");
        
        if (autoScanOnBind && _pos != null && item != null)
            _pos.AutoScan(item);     // 드랍되자마자 합계 반영

    }

    private void OnMouseDown()
    {
        if (!autoScanOnBind && _pos != null && item != null)
            _pos.AutoScan(item);
    }
}
