using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSequence : MonoBehaviour
{
    [Header("UI")]
    public Canvas forUndergroundCanvas;   // 처음에 보이는 ForUnderground Canvas
    public GameObject endingUICanvas;     // 사장님 이후에 나올 새 UI Canvas
    public GameObject goHomeButton;       // Go Home 버튼

    [Header("Manager NPC")]
    public GameObject managerNPC;         // 사장님 오브젝트
    public Transform managerSpawnPoint;   // 플레이어 앞 위치

    [Header("Timing")]
    public float delayBeforeGoHome = 1f;  // 새 UI 등장 후 Go Home 버튼까지 딜레이
    public string nextSceneName = "HomeScene";

    [Header("Audio")]
    public AudioSource bgmSource;         // 지금 배경 음악 재생 중인 AudioSource
    public AudioSource sfxSource;         // 효과음 재생용 AudioSource
    public AudioClip clickSFX;            // 첫 버튼 눌렀을 때 나는 효과음
    public AudioClip uiAppearSFX;         // 새 UI 등장할 때 나는 효과음

    void Start()
    {
        if (managerNPC != null)
            managerNPC.SetActive(false);

        if (endingUICanvas != null)
            endingUICanvas.SetActive(false);

        if (goHomeButton != null)
            goHomeButton.SetActive(false);
    }

    // ForUnderground Canvas 안 버튼의 OnClick에서 호출
    public void OnButtonClicked()
    {
        // 1) 기존 Canvas 바로 끄기
        if (forUndergroundCanvas != null)
            forUndergroundCanvas.gameObject.SetActive(false);

        // 2) BGM 멈추기
        if (bgmSource != null)
            bgmSource.Stop();

        // 3) SFX + 딜레이 + 이후 연출 시작
        StartCoroutine(BeginEndingSequence());
    }

    private IEnumerator BeginEndingSequence()
    {
        float waitTime = 0f;

        // (1) 첫 번째 클릭 SFX 재생
        if (sfxSource != null && clickSFX != null)
        {
            sfxSource.PlayOneShot(clickSFX);
            waitTime = clickSFX.length;
        }

        // (2) SFX 길이만큼 기다리기
        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        // (3) 사장님 + 새 UI 등장
        ShowManagerAndUI();

        // (4) Go Home 버튼은 추가 딜레이 후 등장
        if (delayBeforeGoHome > 0f)
            yield return new WaitForSeconds(delayBeforeGoHome);

        ShowGoHomeButton();
    }

    void ShowManagerAndUI()
    {
        // 사장님 활성화
        if (managerNPC != null)
        {
            managerNPC.SetActive(true);

            if (managerSpawnPoint != null)
            {
                managerNPC.transform.position = managerSpawnPoint.position;
                managerNPC.transform.rotation = managerSpawnPoint.rotation;
            }
        }

        // 새 UI Canvas 활성화
        if (endingUICanvas != null)
            endingUICanvas.SetActive(true);

        // 새 UI 등장 SFX 재생
        if (sfxSource != null && uiAppearSFX != null)
            sfxSource.PlayOneShot(uiAppearSFX);
    }

    void ShowGoHomeButton()
    {
        if (goHomeButton != null)
            goHomeButton.SetActive(true);
    }

    // Go Home 버튼 OnClick에서 호출
    public void OnGoHomeButtonClicked()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
