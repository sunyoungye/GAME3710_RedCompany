using System.Collections.Generic;
using UnityEngine;

public class CustomerQueueManager : MonoBehaviour
{
    [Header("Refs")]
    public PosManagerUI pos;
    public Transform spawnPoint, counterPoint, exitPoint;
    public Transform[] counterSlots;
    public Transform[] queueSpots;

    [Header("Prefabs/Data")]
    public GameObject npcPrefab;
    public GameObject cardPrefab;
    public Transform cardSpawnPoint;

    public List<ItemSO> catalog = new();

    private readonly Queue<List<ItemSO>> _orders = new();
    private readonly List<NpcController> _queue = new();
    private bool counterBusy;

    void Start()
    {
        for (int i = 0; i < 5; i++) _orders.Enqueue(MakeOrder(Random.Range(2, 5)));

        // ★ PosManagerUI의 이벤트 구독 (이름 반드시 같아야 함)
        pos.OnOrderPaid += OnOrderPaid;

        SpawnToQueue(3);
        TryDispatch();
    }

    void OnDestroy()
    {
        if (pos) pos.OnOrderPaid -= OnOrderPaid;
    }

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

            int spotIndex = Mathf.Min(_queue.Count, queueSpots.Length - 1);
            npc.agent.Warp(spawnPoint.position);
            npc.agent.avoidancePriority = 50 + _queue.Count;
            npc.Init(pos, counterPoint, exitPoint, counterSlots, order);
            npc.agent.SetDestination(queueSpots[spotIndex].position);
            _queue.Add(npc);
        }
    }

    void TryDispatch()
    {
        if (counterBusy || _queue.Count == 0)
        {
            if (_orders.Count > 0) SpawnToQueue(1);
            return;
        }

        counterBusy = true;
        var npc = _queue[0];
        _queue.RemoveAt(0);

        for (int i = 0; i < _queue.Count; i++)
            _queue[i].agent.SetDestination(queueSpots[i].position);

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
