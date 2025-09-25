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
    public Transform cardSpawnPoint;

    private PosManagerUI _pos;
    private readonly List<GameObject> _spawnedItems = new();

    // Ŭ�� ó���ؾ� �� ���� ������ ��
    private int _remainingToScan = 0;

    // ���� �� �ִ� ī��(�ߺ� ��ȯ ����)
    private GameObject _cardInstance;

    public void Init(PosManagerUI pos, Transform counter, Transform exit, Transform[] slots, List<ItemSO> order)
    {
        _pos = pos;
        counterPoint = counter;
        exitPoint = exit;
        counterSlots = slots;
        itemsToBuy = order;
    }

    public void BeginFlow() => StartCoroutine(Flow());

    private IEnumerator Flow()
    {
        // 1) ī���ͷ� ����
        agent.isStopped = false;
        agent.SetDestination(counterPoint.position);
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f);

        // 2) ������ ���(= Ŭ�� ���)
        yield return DropItemsRandom();

        // 3) POS�� "Ȱ�� ��"���� �ڽ� ���
        _pos.SetActiveCustomer(this);

        // 4) ���� �Ϸ���� ��� (PosManagerUI�� ���� �Ϸ� ������ �̺�Ʈ�� �˷���)
        bool paid = false;
        void OnPaid(NpcController who) { if (who == this) paid = true; }
        _pos.OnOrderPaid += OnPaid;

        yield return new WaitUntil(() => paid);
        _pos.OnOrderPaid -= OnPaid;

        // 5) ����
        agent.isStopped = false;
        agent.SetDestination(exitPoint.position);
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f);

        Destroy(gameObject);
    }

    private IEnumerator DropItemsRandom()
    {
        // ī���� ���� �ε��� ����
        var indices = new List<int>();
        for (int i = 0; i < counterSlots.Length; i++) indices.Add(i);
        for (int i = 0; i < indices.Count; i++)
        {
            int j = Random.Range(i, indices.Count);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        _spawnedItems.Clear();
        _remainingToScan = Mathf.Min(itemsToBuy.Count, counterSlots.Length);

        for (int i = 0; i < _remainingToScan; i++)
        {
            ItemSO so = itemsToBuy[i];
            Transform slot = counterSlots[indices[i]];

            GameObject go = Instantiate(so.prefab, slot.position, slot.rotation);
            var view = go.GetComponent<ItemView>();
            if (view == null) view = go.AddComponent<ItemView>();
            view.Bind(so, _pos, this);   // �� 3�� ���� ���ε�

            _spawnedItems.Add(go);
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>ItemView�� Ŭ���Ǿ� POS�� �ݿ��Ǿ��� ������ ȣ��</summary>
    public void OneItemScanned()
    {
        _remainingToScan = Mathf.Max(0, _remainingToScan - 1);
        if (_remainingToScan == 0)
        {
            SpawnCard();
        }
    }

    private void SpawnCard()
    {
        if (_cardInstance != null || cardPrefab == null || cardSpawnPoint == null) return;

        _cardInstance = Instantiate(cardPrefab, cardSpawnPoint.position, cardSpawnPoint.rotation);
        var token = _cardInstance.GetComponent<CardToken>();
        if (token == null) token = _cardInstance.AddComponent<CardToken>();
        token.Init(this); // ī�忡 "���� ������" �±�
    }
}
