using UnityEngine;
using System.Collections;
using Cinemachine;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    public List<GameObject> HeartsUi = new List<GameObject>();
    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false;
    public bool isDead = false;
    public float invincibilityTime = 1f;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;
    public Color damageColor = Color.white;
    public Vector3 defaultSpawnLocation;
    public GameObject UpperBody;
    public GameObject LowerBody;
    public Sprite Death;
    public SpriteRenderer SpriteRendererUpperBody;
    public SpriteRenderer SpriteRendererLowerBody;
    private SpriteRenderer PlayerSpriteRenderer;
    private Animator PlayerAnimator;
    private Rigidbody2D rb;
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private Obelisk lastObelisk;
    public ScreenFader screenFader;
    public Collider2D VerticalCollider;
    public Collider2D HorizontalCollider;
    private BossController bossController;
    public CinemachineVirtualCamera DefaultCamera;
    private void Start()
    {
        bossController = FindAnyObjectByType<BossController>();
        PlayerAnimator = GetComponent<Animator>();
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();

        UpdateHeartsUI();
    }

    public void UpdateHeartsUI()
    {
        currentHealth = maxHealth;
        for (int i = 0; i < maxHealth; i++)
        {
            HeartsUi[i].SetActive(true);
        }
    }

    // public void Heal()
    // {
    //     if (isInvincible || isDead || currentHealth == maxHealth) return;

    //     currentHealth++;
    // }

    public void TakeDamage(Vector2 attackDirection)
    {
        if (isInvincible || isDead || playerMovement.IsDashing) return;

        currentHealth--;
        HeartsUi[currentHealth].SetActive(false);
        SoundManager.Instance.PlaySound(SoundManager.SoundID.HeroDamage, worldPos: transform.position, volumeUpdate: 0.2f);
        if (currentHealth <= 0)
        {
            isDead = true;
            StartCoroutine(bossController.RestartBossBattle());
            CameraManager.Instance.ReturnCameraToDefault(DefaultCamera);
            UpperBody.SetActive(false);
            LowerBody.SetActive(false);

            PlayerAnimator.Play("Death");

            Time.timeScale = 0.5f;

            VerticalCollider.enabled = false;
            HorizontalCollider.enabled = true;

            StartCoroutine(Knockback(attackDirection));
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
            SpriteRendererUpperBody.color = damageColor;
            SpriteRendererLowerBody.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            
            SpriteRendererUpperBody.color = Color.white;
            SpriteRendererLowerBody.color = Color.white;
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

        // yield return new WaitForSeconds(knockbackDuration);
        yield return new WaitForSecondsRealtime(knockbackDuration);
        if (!isDead)
        {
            playerMovement.enabled = true;
            playerAttack.enabled = true;
        }
        else
        {
            Die();
        }
    }

    private IEnumerator InvincibilityFrames()
    {
        
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
        StopCoroutine(FlashWhite());
    }

    private void Die()
    {
        // if (isDead) return;
        
        foreach (MonsterScript enemy in MonsterScript.ActiveMonsters)
        {
            enemy.StopEnemy();
        }  

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        screenFader.FadeToWhite();

        yield return new WaitForSecondsRealtime(1f);
        // PlayerSpriteRenderer.sprite = null;
        PlayerAnimator.Play("None");
        UpperBody.SetActive(true);
        LowerBody.SetActive(true);
        Time.timeScale = 1f;
        VerticalCollider.enabled = true;
        HorizontalCollider.enabled = false;
        if (lastObelisk != null)
        {
            // Debug.Log("lastObelisk " + lastObelisk.obeliskID);
            transform.position = lastObelisk.GetSpawnPoint();
        }
        else
        {
            transform.position = defaultSpawnLocation;
        }
        UpdateHeartsUI();
        playerAttack.enabled = true;
        playerMovement.enabled = true;
        isDead = false;
        
        foreach (MonsterScript enemy in MonsterScript.ActiveMonsters)
        {
            enemy.StartEnemy();
        }  
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(screenFader.FadeFromWhite());
    }

    public void SetLastObelisk(Obelisk obelisk)
    {
        lastObelisk = obelisk;
    }
}
