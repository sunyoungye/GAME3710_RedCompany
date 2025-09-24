using UnityEngine;

[CreateAssetMenu(menuName = "POS/Item")]
public class ItemSO : ScriptableObject
{
    public string sku = "SKU-0001";
    public string displayName = "Apple";
    [Min(0)] public int priceCents = 199;
    [Tooltip("테이블 위에 놓일 프리팹(반드시 Collider 포함)")]
    public GameObject prefab;
}
