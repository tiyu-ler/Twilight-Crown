using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorHideScript : MonoBehaviour
{
    public enum EntrySide { Right, Bottom }
    public EntrySide allowedEntrySide = EntrySide.Right;
    public SpriteRenderer[] _childSprites;
    void Start()
    {
        SetOpacity(0f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("111");
        Debug.Log(other.tag);
        if (!other.CompareTag("Player")) return;
        Debug.Log("333");
        Vector3 entryDirection = other.transform.position - transform.position;

        if (allowedEntrySide == EntrySide.Right)
        {
            if (entryDirection.x < 0) SetOpacity(0f);
            else SetOpacity(255f);
        }
        else if (allowedEntrySide == EntrySide.Bottom && entryDirection.z < 0)
            SetOpacity(255f);
    }

    public void RevealHiddenObjects()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInChildren(0.8f));
    }

    private void SetOpacity(float alpha)
    {
        foreach (SpriteRenderer sr in _childSprites)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    private IEnumerator FadeInChildren(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / duration);

            foreach (SpriteRenderer sr in _childSprites)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure full opacity at the end
        foreach (SpriteRenderer sr in _childSprites)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
    }
}

