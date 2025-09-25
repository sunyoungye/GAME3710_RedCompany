using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardReader : MonoBehaviour
{
    [SerializeField] private PosManagerUI pos;

    private void Reset()
    {
        // Collider를 Trigger로 만들어두면 편함
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var token = other.GetComponent<CardToken>();
        if (token == null || pos == null) return;

        pos.OnCardAcceptedByReader(token.owner);
        Destroy(token.gameObject);
    }
}
