using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CartLine
{
    public ItemSO item;
    public int qty;
    public int Line => item.priceCents * qty;
}

public class PosManagerUI : MonoBehaviour
{
    public event Action<NpcController> OnOrderPaid;

    [Header("UI")]
    public PosUI ui;                          // (기존에 쓰던 UI 바인딩용 스크립트)
    public GameObject paymentCompletePanel;   // 선택 항목

    private readonly List<CartLine> _cart = new();
    private NpcController _current;

    private void Awake()
    {
        if (paymentCompletePanel) paymentCompletePanel.SetActive(false);
    }

    public void SetActiveCustomer(NpcController npc)
    {
        // 같은 손님이면 다시 안내만
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

    /// <summary>아이템 클릭 시 POS 장바구니에 1개 라인 추가</summary>
    public void AddItemLine(ItemSO item)
    {
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

    // 카드리더기가 승인 신호를 줄 때 호출(Reader가 호출)
    public void OnCardAcceptedByReader(NpcController who)
    {
        if (who != _current) return;

        ui?.SetReceipt(BuildReceipt());
        ui?.SetStatus("Payment complete!");
        _cart.Clear();
        RefreshUI();

        if (paymentCompletePanel) paymentCompletePanel.SetActive(true);

        var c = _current;
        _current = null;
        OnOrderPaid?.Invoke(c);   // QueueManager/NPC 쪽에 결제 완료 신호
    }

    private int Subtotal() => _cart.Sum(l => l.Line);

    private void RefreshUI()
    {
        ui?.SetSubtotalCents(Subtotal());
        ui?.RebuildItemLines(_cart.Select(l => (l.item.displayName, l.qty, l.Line)).ToList());
    }

    private string BuildReceipt()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== RECEIPT ===");
        foreach (var l in _cart)
            sb.AppendLine($"{l.item.displayName} x{l.qty}  {l.Line / 100f:0.00}");
        sb.AppendLine("----------------");
        sb.AppendLine($"TOTAL: {Subtotal() / 100f:0.00}");
        return sb.ToString();
    }
}
