using UnityEngine;

public class SimpleGrabber : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;


    public Transform handAnchor;     // 카메라 앞 손 위치
    [SerializeField] private PosManagerUI pos; // ✅ 추가: POS 참조

    private CardToken holding;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!pos) pos = FindFirstObjectByType<PosManagerUI>();
    }

    void Update()
    {
        // 마우스 왼쪽 클릭
        if (Input.GetMouseButtonDown(0))
        {
            if (holding == null) TryPick();
            else TryDrop();
        }
    }

    void TryPick()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int mask = ~LayerMask.GetMask("Player");
        if (Physics.Raycast(ray, out var hit, 100f, mask, QueryTriggerInteraction.Ignore))
        {
            var card = hit.collider.GetComponentInParent<CardToken>();
            if (card)
            {
                holding = card;
                card.PickUp(handAnchor);

                // ✅ 변경 핵심: Find로 POS 찾지 말고, "이 카드의 pos"로 결제 호출
                if (card.pos != null && card.owner != null)
                {
                    card.pos.OnCardAcceptedByReader(card.owner);
                }

                Destroy(card.gameObject);
                holding = null;
            }
        }
    }


    void TryDrop()
    {
        // 안전망: 혹시 카드가 남아있다면 일반 드롭
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            holding.Drop(hit.point);
            holding = null;
        }
    }
}
