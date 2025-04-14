using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIButtonLightUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject lightUp = null;

    public bool affectedByMouseClick = false;
    public bool AffectsButtonDirectly;
    public TextMeshProUGUI lightImage = null;
    public float lerpDuration = 0.3f;
    public CanvasGroup canvasGroup = null;
    private Vector3 smallScale = new Vector3(0.85f, 0.85f, 1f);
    private Vector3 largeScale = Vector3.one;
    private Vector3 mediumScale = new Vector3(0.9f, 0.9f, 1f);
    private Color startColor = new Color(92 / 255f, 110 / 255f, 215 / 255f, 0); //rgba(92, 110, 215, 0)
    private Color endColor = new Color(59f / 255f, 74f / 255f, 161f / 255f, 1);   // #3B4AA1
    private Coroutine animationCoroutine;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && affectedByMouseClick) 
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateLightUp(smallScale, startColor, true));
        }
    }
    void Start()
    {
        if (!AffectsButtonDirectly)
        {
            lightUp.SetActive(false);
            lightUp.transform.localScale = smallScale;

            lightImage.color = startColor;
        }
        else
        {
            canvasGroup.transform.localScale = smallScale;

            canvasGroup.alpha = 0.85f;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!AffectsButtonDirectly)
        {
            SoundManager.Instance.PlaySound(SoundManager.SoundID.UiButtonSelection, volumeUpdate: 0.002f, pitch: 2f);
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            lightUp.SetActive(true);
            animationCoroutine = StartCoroutine(AnimateLightUp(largeScale, endColor));
        }
        else
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(CanvasGroupLightUp(mediumScale, 1f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!AffectsButtonDirectly)
        {
            // SoundManager.Instance.PlaySound(SoundManager.SoundID.UiButtonSelection, volumeUpdate: 0.002f, pitch: -0.1f);
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateLightUp(smallScale, startColor, true));
        }
        else
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(CanvasGroupLightUp(smallScale, 0.75f));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!AffectsButtonDirectly)
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateLightUp(smallScale, startColor, true));
        }
        else
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(CanvasGroupLightUp(smallScale, 0.75f));
        }
    }
    
    private IEnumerator AnimateLightUp(Vector3 targetScale, Color targetColor, bool deactivateAfter = false)
    {
        float time = 0;
        Vector3 startScale = lightUp.transform.localScale;
        Color startCol = lightImage.color;
        
        while (time < lerpDuration)
        {
            time += Time.deltaTime;
            float t = time / lerpDuration;
            lightUp.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            lightImage.color = Color.Lerp(startCol, targetColor, t);
            yield return null;
        }

        lightUp.transform.localScale = targetScale;
        lightImage.color = targetColor;

        if (deactivateAfter)
        {
            yield return new WaitForSeconds(0.1f); // Small delay before deactivating
            if (!AffectsButtonDirectly)
            {
                lightUp.SetActive(false);
            }
        }
    }

    private IEnumerator CanvasGroupLightUp(Vector3 targetScale, float targetAlpha, bool deactivateAfter = false)
    {
        float time = 0;
        Vector3 startScale = canvasGroup.transform.localScale;
        float startAlpha = canvasGroup.alpha;

        while (time < lerpDuration)
        {
            time += Time.deltaTime;
            float t = time / lerpDuration;
            canvasGroup.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.transform.localScale = targetScale;
        canvasGroup.alpha = targetAlpha;
    }
}
