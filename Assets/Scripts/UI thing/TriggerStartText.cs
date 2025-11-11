using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerStartText : MonoBehaviour
{
    [SerializeField] private TextAnim textAnim;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool playOnce = true;

    private bool _alreadyPlayed;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_alreadyPlayed && playOnce) return;
        if (other.CompareTag(targetTag))
        {
            if (textAnim != null)
            {
                textAnim.Play();
                _alreadyPlayed = true;
            }
            else
            {
                Debug.LogWarning("[TriggerStartText] TextAnim reference is empty.");
            }
        }
    }
}
