using UnityEngine;

public class MonitorClickSound : MonoBehaviour
{
    [Header("Quest Settings")]
    [SerializeField] private Quest quest;
    [SerializeField] private int stepIndexToComplete = 0;

    [Header("Sound")]
    public AudioSource audioSource;   // 사운드 재생할 AudioSource
    public AudioClip clickClip;       // 모니터 클릭 시 재생할 클립
    public bool playOnce = false;     // 한 번만 재생할지 여부

    private bool _alreadyPlayed = false;

    void OnMouseDown()
    {
        // 이미 한 번 재생했고, playOnce 가 true면 더 이상 재생 안 함
        if (playOnce && _alreadyPlayed) return;

        if (audioSource != null && clickClip != null)
        {
            if (quest != null)
                quest.CompleteStep(stepIndexToComplete);
            else
                Debug.LogWarning("Look at the Quest!.");

            audioSource.PlayOneShot(clickClip);
            _alreadyPlayed = true;
        }
    }
}
