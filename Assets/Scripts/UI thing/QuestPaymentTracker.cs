using System.Collections.Generic;
using UnityEngine;

public class QuestPaymentTracker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PosManagerUI pos;   
    [SerializeField] private Quest quest;        
    [SerializeField] private int stepIndexToComplete = 0; 
    [SerializeField] private int targetPaidCount = 5;     

    private readonly HashSet<NpcController> _uniquePaid = new();

    private void OnEnable()
    {
        if (pos != null)
            pos.OnOrderPaid += HandlePaid;
    }

    private void OnDisable()
    {
        if (pos != null)
            pos.OnOrderPaid -= HandlePaid;
    }

    private void HandlePaid(NpcController who)
    {
        if (who == null) return;

        if (_uniquePaid.Add(who))
        {
            if (_uniquePaid.Count >= targetPaidCount && quest != null)
            {
                quest.CompleteStep(stepIndexToComplete);
                // 한 번만 작동하도록 막고 싶으면 주석 해제:
                // pos.OnOrderPaid -= HandlePaid;
            }
        }
    }
}
