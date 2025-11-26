using System.Collections;
using UnityEngine;
using TMPro;

public class TextAnim : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    [TextArea] public string[] stringArray;
    [SerializeField] private float timeBtwnChars = 0.03f;
    [SerializeField] private float timeBtwnWords = 0.8f;

    private int i = 0;
    private bool _isPlaying = false;

    public event System.Action OnFinished;

    public void Play()
    {
        if (_isPlaying) return;
        _isPlaying = true;
        i = 0;
        StopAllCoroutines();
        EndCheck();
    }

    private void EndCheck()
    {
        if (i <= stringArray.Length - 1)
        {
            _textMeshPro.text = stringArray[i];
            _textMeshPro.maxVisibleCharacters = 0;
            StartCoroutine(TextVisible());
        }
        else
        {
            _isPlaying = false;
            OnFinished?.Invoke();
        }
    }

    private IEnumerator TextVisible()
    {
        _textMeshPro.ForceMeshUpdate();
        int totalVisibleCharacters = _textMeshPro.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            _textMeshPro.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalVisibleCharacters)
            {
                i += 1;
                yield return new WaitForSeconds(timeBtwnWords);
                EndCheck();
                yield break;
            }

            counter += 1;
            yield return new WaitForSeconds(timeBtwnChars);
        }
    }
}
