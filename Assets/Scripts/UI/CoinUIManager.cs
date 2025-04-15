using System.Collections;
using TMPro;
using UnityEngine;

public class CoinUIManager : MonoBehaviour
{
    public TextMeshProUGUI MoneyText;
    public CanvasGroup Coins;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (MoneyText) MoneyText.text = PlayerDataSave.Instance.Money.ToString();
    }
    public void UpdateCoinsUI(int money)
    {
        MoneyText.text = money.ToString();
        StartCoroutine(FadeCanvasGroup(Coins, 0.08f, 1f));

        if (hideCoroutine == null)
            hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(1);

        StartCoroutine(FadeCanvasGroup(Coins, 0.08f, 0f));
        yield return StartCoroutine(FadeCanvasGroup(Coins, 0.08f, 0f));
        hideCoroutine = null;
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
}
