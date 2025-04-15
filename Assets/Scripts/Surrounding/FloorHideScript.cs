using System.Collections;
using UnityEngine;

public class FloorHideScript : MonoBehaviour
{
    public bool SideWays;
    public SpriteRenderer[] _childSprites;
    private bool _sphereFlyActivated = false;
    private EndGameSphere endGameSphere;
    void Start()
    {
        endGameSphere = FindAnyObjectByType<EndGameSphere>();
        SetOpacity(255f);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 entryDirection = other.transform.position - transform.position;

        if (SideWays)
        {
            if (entryDirection.x > 0) 
            {
                SetOpacity(0f);
                if (!_sphereFlyActivated) 
                {
                    _sphereFlyActivated = true;
                    endGameSphere.StartCoroutine(endGameSphere.SphereFly());
                }
            }
            else SetOpacity(255f);
        }
        else StartCoroutine(FadeInChildren(1f));
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

        foreach (SpriteRenderer sr in _childSprites)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
    }
}

