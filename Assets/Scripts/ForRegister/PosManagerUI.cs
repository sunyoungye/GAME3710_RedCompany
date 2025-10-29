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
    public PosUI ui;                          
    public GameObject paymentCompletePanel;  

    private readonly List<CartLine> _cart = new();
    private NpcController _current;

    private void Awake()
    {
        if (paymentCompletePanel) paymentCompletePanel.SetActive(false);
    }

    public void SetActiveCustomer(NpcController npc)
    {
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
        OnOrderPaid?.Invoke(c);   
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
