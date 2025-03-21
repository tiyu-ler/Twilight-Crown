using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Animator BulletAnimator;
    public float FallTime;
    private Collider2D _collider;
    void Start()
    {
        BulletAnimator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        StartCoroutine(BulletFall());
    }

    private IEnumerator BulletFall()
    {
        float elapsedTime  = 0f;
        Vector2 startPos = transform.localPosition;
        Vector2 endPos = new Vector3(startPos.x, -1.22f);

        
        while (elapsedTime < FallTime)
        {
            elapsedTime += Time.deltaTime;

            transform.localPosition = Vector2.Lerp(startPos, endPos, elapsedTime/FallTime);
            yield return null;
        }

        transform.localPosition = endPos;
        _collider.enabled = false;
        BulletAnimator.Play("BulletSplash");
        yield return new WaitForSeconds(GetAnimationLength("BulletSplash"));
        Destroy(gameObject);
    }

    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = BulletAnimator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
                return clip.length;
        }
        return 0.1f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Vector2 attackDirection = (transform.position - collision.transform.position).normalized;
                playerHealth.TakeDamage(attackDirection);
            }
        }
    }
}
