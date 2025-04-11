using System.Collections;
using UnityEngine;
using TMPro;

public class MarkerTextPopUp : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _popUpText;
    private Vector3 _originalScale;
    private bool _isActive;
    void Start()
    {
        _isActive = true;

        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _popUpText = GetComponentInChildren<TextMeshProUGUI>();

        _canvasGroup.alpha = 0;
        _originalScale = _popUpText.transform.localScale;
        _popUpText.transform.localScale = _originalScale * 0.8f;
        
        Color color = _popUpText.color;
        color.a = 0f;
        _popUpText.color = color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActive)
        {
            StopAllCoroutines();
            try {
            StartCoroutine(PopUp());
            } catch {}
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isActive)
        {
            StopAllCoroutines();
            try{
            StartCoroutine(Hide());
            } catch {}
        }
    }

    private IEnumerator PopUp()
    {
        yield return StartCoroutine(FadeText(_popUpText, 0.08f, 1f, _originalScale));
        yield return StartCoroutine(FadeCanvasGroup(_canvasGroup, 0.08f, 1f));
    }

    private IEnumerator Hide()
    {
        yield return StartCoroutine(FadeCanvasGroup(_canvasGroup, 0.08f, 0f));
        yield return StartCoroutine(FadeText(_popUpText, 0.08f, 0f, _originalScale * 0.8f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float duration, float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float duration, float targetAlpha, Vector3 targetScale)
    {
        float startAlpha = text.color.a;
        Color color = text.color;
        Vector3 startScale = text.transform.localScale;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            text.color = color;
            text.transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            yield return null;
        }

        color.a = targetAlpha;
        text.color = color;
        text.transform.localScale = targetScale;
    }

    public void ForcedPopUp()
    {
        ActivateMarkUp();
        StopAllCoroutines();
        StartCoroutine(PopUp());
    }
    public void DisableMarkUp()
    {
        _isActive = false;
        StopAllCoroutines();
        StartCoroutine(Hide());
    }

    public void ActivateMarkUp()
    {
        _isActive = true;
    }
}
