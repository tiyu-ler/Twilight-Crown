using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false;
    private bool isDead = false;

    public float invincibilityTime = 1f;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;
    public Color damageColor = Color.white;
    // public Image[] heartsUI;
    public Vector3 defaultSpawnLocation;
    public Animator UpperBodyAnimator;
    public Animator LowerBodyAnimator;
    public SpriteRenderer spriteRendererUpperBody;
    public SpriteRenderer spriteRendererLowerBody;
    private Rigidbody2D rb;
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private Obelisk lastObelisk;
    public ScreenFader screenFader;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();

        // UpdateHeartsUI();
    }

    public void Heal()
    {
        if (isInvincible || isDead || currentHealth == maxHealth) return;

        currentHealth++;
    }

    public void TakeDamage(Vector2 attackDirection)
    {
        if (isInvincible || isDead) return;

        currentHealth--;
        // UpdateHeartsUI();
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            playerAttack.enabled = false;
            playerMovement.enabled = false;
            Die();
        }
        else
        {
            isInvincible = true;
            StartCoroutine(FlashWhite());
            StartCoroutine(Knockback(attackDirection));
            StartCoroutine(InvincibilityFrames());
        }
    }

    private IEnumerator FlashWhite()
{
    while (isInvincible)
    {
        spriteRendererUpperBody.color = damageColor;
        spriteRendererLowerBody.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        
        spriteRendererUpperBody.color = Color.white;
        spriteRendererLowerBody.color = Color.white;
        yield return new WaitForSeconds(0.1f);
    }
}

    private IEnumerator Knockback(Vector2 attackDirection)
    {
        playerAttack.enabled = false;
        playerMovement.enabled = false;
        rb.velocity = Vector2.zero;

        Vector2 knockback = new Vector2(-attackDirection.x, 0.25f).normalized * knockbackForce;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);
        playerMovement.enabled = true;
        playerAttack.enabled = true;
    }

    private IEnumerator InvincibilityFrames()
    {
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
        StopCoroutine(FlashWhite());
    }

    private void Die()
    {
        if (isDead) return;
        
        foreach (MonsterScript enemy in MonsterScript.ActiveMonsters)
        {
            enemy.StopEnemy();
        }  

        isDead = true;
        // UpperBodyAnimator.Play("Death");
        // LowerBodyAnimator.Play("Death");
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);
        screenFader.FadeToWhite();

        yield return new WaitForSeconds(1f);
        if (lastObelisk != null)
        {
            Debug.Log("lastObelisk " + lastObelisk.obeliskID);
            transform.position = lastObelisk.GetSpawnPoint();
        }
        else
        {
            transform.position = defaultSpawnLocation;
        }

        currentHealth = maxHealth;
        // UpdateHeartsUI();
        isDead = false;
        screenFader.FadeFromWhite();
    }

    public void SetLastObelisk(Obelisk obelisk)
    {
        lastObelisk = obelisk;
    }

    // private void UpdateHeartsUI()
    // {
    //     for (int i = 0; i < heartsUI.Length; i++)
    //     {
    //         heartsUI[i].enabled = i < currentHealth;
    //     }
    // }
}
