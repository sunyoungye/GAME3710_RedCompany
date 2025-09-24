using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CartLine
{
    public ItemSO item;
    public int qty;
    public int Line => item.priceCents * qty; // cents
}

public class PosManagerUI : MonoBehaviour
{
    public event Action<NpcController> OnOrderPaid;

    [Header("UI")]
    public PosUI ui;                           // PosUI ������Ʈ
    public GameObject paymentCompletePanel;    // �ɼ�(�Ϸ� �г�)

    private readonly List<CartLine> _cart = new();
    private NpcController _current;

    public NpcController CurrentCustomer => _current; // ī�� ������ ������

    void Awake()
    {
        if (paymentCompletePanel) paymentCompletePanel.SetActive(false);
    }

    public void SetActiveCustomer(NpcController npc)
    {
        Debug.Log($"[PosManagerUI] SetActiveCustomer({npc?.name})");

        // ���� �մ��̸� �ʱ�ȭ ���� (�ڵ� ��ĵ ���� 0���� �������� ���� ����)
        if (_current == npc)
        {
            ui?.SetStatus("Place items, then tap card on reader.");
            if (paymentCompletePanel) paymentCompletePanel.SetActive(false);
            return;
        }

        _current = npc;
        _cart.Clear();
        RefreshUI();
        ui?.SetStatus("Place items, then tap card on reader.");
        ui?.SetReceipt("");
        if (paymentCompletePanel) paymentCompletePanel.SetActive(false);
    }

    // �������� ���̺� ���� �� �ڵ� ȣ��
    public void AutoScan(ItemSO item)
    {
        Debug.Log($"[PosManagerUI.AutoScan] {item?.displayName}  price={item?.priceCents}");
        if (item == null) return;

        var line = _cart.FirstOrDefault(l => l.item == item);
        if (line == null)
        {
            line = new CartLine { item = item, qty = 0 };
            _cart.Add(line);
        }
        line.qty++;
        RefreshUI();
    }

    // ī�� �����Ⱑ ���� ��ȣ�� �� �� ȣ��
    public void OnCardAcceptedByReader(NpcController who)
    {
        if (who != _current) return;

        ui?.SetReceipt(BuildReceipt());
        ui?.SetStatus("Payment complete!");
        _cart.Clear();
        RefreshUI();
        if (paymentCompletePanel) paymentCompletePanel.SetActive(true);

        var paid = _current;
        _current = null;
        OnOrderPaid?.Invoke(paid); // NPC���� "���� ��" �˸�
    }

    int SubtotalCents() => _cart.Sum(l => l.Line);

    void RefreshUI()
    {
        var subtotal = SubtotalCents();
        ui?.SetSubtotalCents(subtotal);
        ui?.RebuildItemLines(_cart.Select(l => (l.item.displayName, l.qty, l.Line)).ToList());
    }

    string BuildReceipt()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== RECEIPT ===");
        foreach (var l in _cart)
            sb.AppendLine($"{l.item.displayName} x{l.qty}  {(l.Line / 100f):0.00}");
        sb.AppendLine("----------------");
        sb.AppendLine($"TOTAL: {(SubtotalCents() / 100f):0.00}");
        return sb.ToString();
    }

    // (�ʿ� ��) �ܺο��� ���� �ʱ�ȭ�� �� ���
    public void ClearSale()
    {
        Debug.Log("[PosManagerUI] ClearSale()");
        _cart.Clear();
        _current = null;
        RefreshUI();
        ui?.SetStatus("Waiting...");
        if (paymentCompletePanel) paymentCompletePanel.SetActive(false);
    }
}
