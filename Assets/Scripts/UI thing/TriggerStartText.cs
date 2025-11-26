using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerStartText : MonoBehaviour
{
    [SerializeField] private TextAnim textAnim;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool playOnce = true;
    [SerializeField] private float hideDelay = 0.5f;

    private bool _alreadyPlayed;

    void OnEnable()
    {
        if (textAnim != null)
            textAnim.OnFinished += HandleFinished;
    }

    void OnDisable()
    {
        if (textAnim != null)
            textAnim.OnFinished -= HandleFinished;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_alreadyPlayed && playOnce) return;
        if (other.CompareTag(targetTag))
        {
            _alreadyPlayed = true;
            textAnim.Play();
        }
    }

    void HandleFinished()
    {
        StartCoroutine(HideRoutine());
    }

    System.Collections.IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(hideDelay);
        textAnim.gameObject.SetActive(false);
    }
}
