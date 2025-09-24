using UnityEngine;

[CreateAssetMenu(menuName = "POS/Item")]
public class ItemSO : ScriptableObject
{
    public string sku = "SKU-0001";
    public string displayName = "Apple";
    [Min(0)] public int priceCents = 199;
    [Tooltip("���̺� ���� ���� ������(�ݵ�� Collider ����)")]
    public GameObject prefab;
}
