using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PosUI : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text subtotalText;           // ��Total Amount: 0.00��
    public TMP_Text statusText;             // ���� ����
    public TMP_Text receiptText;            // ������ ����
    public RectTransform itemsContent;      // ��ũ���� Content
    public GameObject itemLinePrefab;       // �ڽ�: NameText / QtyText / PriceText

    class Row { public TMP_Text name, qty, price; public int unitPrice; }
    readonly Dictionary<string, Row> _rows = new();

    public void SetSubtotalCents(int cents)
    {
        Debug.Log($"[PosUI.SetSubtotalCents] subtotal={cents}");
        if (subtotalText) subtotalText.text = $"Total Amount: {(cents / 100f):0.00}";
    }

    public void SetStatus(string msg) => statusText?.SetText(msg);
    public void SetReceipt(string txt) => receiptText?.SetText(txt);

    public void RebuildItemLines(List<(string name, int qty, int lineCents)> lines)
    {
        Debug.Log($"[PosUI.RebuildItemLines] lines={lines.Count}");
        // ���� ����: �Ź� �� ����� �ٽ� �׸� (���� ���)
        foreach (Transform c in itemsContent) Destroy(c.gameObject);
        _rows.Clear();

        foreach (var (name, qty, lineCents) in lines)
        {
            var go = Instantiate(itemLinePrefab, itemsContent);
            var nameT = go.transform.Find("NameText")?.GetComponent<TMP_Text>();
            var qtyT = go.transform.Find("QtyText")?.GetComponent<TMP_Text>();
            var price = go.transform.Find("PriceText")?.GetComponent<TMP_Text>();

            if (nameT) nameT.text = name;
            if (qtyT) qtyT.text = qty.ToString();
            if (price) price.text = (lineCents / 100f).ToString("0.00");
        }
    }
}
