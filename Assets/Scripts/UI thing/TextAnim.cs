using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class TextAnim : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI _textMeshPro;


    [TextArea]
    [SerializeField] private string[] lines;

    [Header("Options")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool playOnce = true;
    [SerializeField] private float timeBtwnChars = 0.03f;
    [SerializeField] private float timeBtwnWords = 0.8f;
    [SerializeField] private float hideDelay = 0.5f;

    private bool _alreadyPlayed = false;
    private bool _isPlaying = false;
    private int _index = 0;

    private void Awake()
    {
        // Trigger 강제
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        // PlayOnce면 이미 재생했으면 무시
        if (_alreadyPlayed && playOnce) return;

        _alreadyPlayed = true;

        // 재생 중이면 무시해야 깔끔함
        if (_isPlaying) return;

        _index = 0;
        _textMeshPro.gameObject.SetActive(true);
        StartCoroutine(PlayTextRoutine());
    }

    private IEnumerator PlayTextRoutine()
    {
        _isPlaying = true;

        while (_index < lines.Length)
        {
            yield return StartCoroutine(TypeLine(lines[_index]));
            _index++;
            yield return new WaitForSeconds(timeBtwnWords);
        }

        yield return new WaitForSeconds(hideDelay);

        _textMeshPro.gameObject.SetActive(false);
        _isPlaying = false;

    }

    private IEnumerator TypeLine(string line)
    {
        _textMeshPro.text = line;
        _textMeshPro.ForceMeshUpdate();

        int total = _textMeshPro.textInfo.characterCount;
        int counter = 0;

        _textMeshPro.maxVisibleCharacters = 0;

        while (counter <= total)
        {
            _textMeshPro.maxVisibleCharacters = counter;
            counter++;
            yield return new WaitForSeconds(timeBtwnChars);
        }
    }
}


