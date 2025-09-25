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

    // 클릭 처리해야 할 남은 아이템 수
    private int _remainingToScan = 0;

    // 현재 떠 있는 카드(중복 소환 방지)
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
        // 1) 카운터로 입장
        agent.isStopped = false;
        agent.SetDestination(counterPoint.position);
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f);

        // 2) 아이템 드랍(= 클릭 대기)
        yield return DropItemsRandom();

        // 3) POS의 "활성 고객"으로 자신 등록
        _pos.SetActiveCustomer(this);

        // 4) 결제 완료까지 대기 (PosManagerUI가 결제 완료 시점에 이벤트로 알려줌)
        bool paid = false;
        void OnPaid(NpcController who) { if (who == this) paid = true; }
        _pos.OnOrderPaid += OnPaid;

        yield return new WaitUntil(() => paid);
        _pos.OnOrderPaid -= OnPaid;

        // 5) 퇴장
        agent.isStopped = false;
        agent.SetDestination(exitPoint.position);
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f);

        Destroy(gameObject);
    }

    private IEnumerator DropItemsRandom()
    {
        // 카운터 슬롯 인덱스 섞기
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
            view.Bind(so, _pos, this);   // ★ 3개 인자 바인딩

            _spawnedItems.Add(go);
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>ItemView가 클릭되어 POS에 반영되었을 때마다 호출</summary>
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
        token.Init(this); // 카드에 "누구 것인지" 태깅
    }
}
