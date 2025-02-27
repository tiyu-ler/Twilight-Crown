using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 1f;

    public void FadeToWhite()
    {
        StartCoroutine(Fade(1f));
    }

    public void FadeFromWhite()
    {
        StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float alpha = fadeImage.color.a;
        while (!Mathf.Approximately(alpha, targetAlpha))
        {
            alpha = Mathf.MoveTowards(alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            fadeImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }
}
