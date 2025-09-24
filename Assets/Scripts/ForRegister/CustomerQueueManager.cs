using System.Collections.Generic;
using UnityEngine;

public class CustomerQueueManager : MonoBehaviour
{
    [Header("Refs")]
    public PosManagerUI pos;
    public Transform spawnPoint, counterPoint, exitPoint;
    public Transform[] counterSlots;     // 테이블 슬롯들
    public Transform[] queueSpots;       // 줄 서는 위치들(앞이 0)

    [Header("Prefabs/Data")]
    public GameObject npcPrefab;         // NpcController + NavMeshAgent 포함
    public GameObject cardPrefab;
    public Transform cardSpawnPoint;

    public List<ItemSO> catalog = new();

    private readonly Queue<List<ItemSO>> _orders = new();
    private readonly List<NpcController> _queue = new();
    private bool counterBusy;

    void Start()
    {
        // 데모 주문 생성
        for (int i = 0; i < 5; i++) _orders.Enqueue(MakeOrder(Random.Range(2, 5)));
        pos.OnOrderPaid += OnOrderPaid;

        SpawnToQueue(3);
        TryDispatch();
    }

    void OnDestroy() { if (pos) pos.OnOrderPaid -= OnOrderPaid; }

    void OnOrderPaid(NpcController who)
    {
        counterBusy = false;
        TryDispatch();
    }

    void SpawnToQueue(int count)
    {
        for (int i = 0; i < count && _orders.Count > 0; i++)
        {
            var order = _orders.Dequeue();
            var go = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
            var npc = go.GetComponent<NpcController>();
            npc.cardPrefab = cardPrefab;
            npc.cardSpawnPoint = cardSpawnPoint;

            // 줄 뒤로 보내기
            int spotIndex = Mathf.Min(_queue.Count, queueSpots.Length - 1);
            npc.agent.Warp(spawnPoint.position);
            npc.agent.avoidancePriority = 50 + _queue.Count; // 서로 비켜가도록
            npc.Init(pos, counterPoint, exitPoint, counterSlots, order);
            npc.agent.SetDestination(queueSpots[spotIndex].position);
            _queue.Add(npc);
        }
    }

    void TryDispatch()
    {
        if (counterBusy || _queue.Count == 0) { if (_orders.Count > 0) SpawnToQueue(1); return; }

        counterBusy = true;
        var npc = _queue[0];
        _queue.RemoveAt(0);

        // 나머지 줄 한 칸씩 땡기기
        for (int i = 0; i < _queue.Count; i++)
            _queue[i].agent.SetDestination(queueSpots[i].position);

        // 카운터로 보내고 흐름 시작
        npc.BeginFlow();

        if (_orders.Count > 0) SpawnToQueue(1);
    }

    List<ItemSO> MakeOrder(int count)
    {
        var list = new List<ItemSO>();
        if (catalog.Count == 0) return list;
        for (int i = 0; i < count; i++) list.Add(catalog[Random.Range(0, catalog.Count)]);
        return list;
    }
}
