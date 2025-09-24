using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardReader : MonoBehaviour
{
    public PosManagerUI pos;

    void OnTriggerEnter(Collider other)
    {
        var card = other.GetComponentInParent<CardToken>();
        if (card == null) return;

        pos.OnCardAcceptedByReader(card.owner); // 승인
        Destroy(card.gameObject);               // 카드 제거
    }
}
