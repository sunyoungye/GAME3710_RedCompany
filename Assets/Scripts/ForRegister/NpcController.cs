using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState { Idle, Entering, Dropping, WaitingPayment, Exiting }

public class NpcController : MonoBehaviour
{
    [Header("Nav")]
    public NavMeshAgent agent;
    public Transform counterPoint;
    public Transform exitPoint;

    [Header("Counter Slots")]
    public Transform[] counterSlots;

    [Header("Order/Catalog")]
    public List<ItemSO> itemsToBuy = new();

    [Header("Card")]
    public GameObject cardPrefab;
    public Transform cardSpawnPoint; // ����ũ �� ī�� �÷��� ��ġ

    private PosManagerUI _pos;
    private readonly List<GameObject> _spawnedItems = new();

    public void Init(PosManagerUI pos, Transform counter, Transform exit, Transform[] slots, List<ItemSO> order)
    {
        _pos = pos; counterPoint = counter; exitPoint = exit; counterSlots = slots; itemsToBuy = order;
    }

    public void BeginFlow() => StartCoroutine(Flow());

    IEnumerator Flow()
    {
        // 1) ī���ͷ� ����
        agent.isStopped = false;
        agent.SetDestination(counterPoint.position);
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f);

        // 2) ���� ���� ���(��� = �ڵ� ��ĵ)
        yield return DropItemsRandom();

        // 3) POS Ȱ�� �� ��� + ī�� ��ȯ
        _pos.SetActiveCustomer(this);
        var cardGO = Instantiate(cardPrefab, cardSpawnPoint.position, cardSpawnPoint.rotation);
        var token = cardGO.GetComponent<CardToken>(); if (token == null) token = cardGO.AddComponent<CardToken>();
        token.Init(this);

        // 4) ���� �Ϸ� ��ٸ�
        bool paid = false;
        void OnPaid(NpcController who) { if (who == this) paid = true; }
        _pos.OnOrderPaid += OnPaid;
        yield return new WaitUntil(() => paid);
        _pos.OnOrderPaid -= OnPaid;

        // 5) �� ������ ġ��� ����
        foreach (var go in _spawnedItems) if (go) Destroy(go);
        agent.isStopped = false;
        agent.SetDestination(exitPoint.position);
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f);

        Destroy(gameObject);
    }

    IEnumerator DropItemsRandom()
    {
        _spawnedItems.Clear();
        var idx = new List<int>();
        for (int i = 0; i < counterSlots.Length; i++) idx.Add(i);
        for (int i = 0; i < idx.Count; i++) { int j = Random.Range(i, idx.Count); (idx[i], idx[j]) = (idx[j], idx[i]); }

        int n = Mathf.Min(itemsToBuy.Count, counterSlots.Length);
        for (int i = 0; i < n; i++)
        {
            var so = itemsToBuy[i];
            var slot = counterSlots[idx[i]];
            var go = Instantiate(so.prefab, slot.position, slot.rotation);
            var view = go.GetComponent<ItemView>(); if (view == null) view = go.AddComponent<ItemView>();
            view.Bind(so, _pos);
            _spawnedItems.Add(go);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
