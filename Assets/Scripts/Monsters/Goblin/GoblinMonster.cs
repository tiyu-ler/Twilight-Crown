using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GoblinMonster : MonsterScript
{
    public GameObject coinPrefab;
    public Sprite died;
    public float DesiredHeight;
    public Transform torch;
    private AudioClip WalkClip, DamageClip;
    public AudioSource audioSource;
    public AudioSource DamageAudioSource;
    private float AmbientVolume, SfxVolume;
    private const float GoblinGetDamageVolume = 0.38f, GoblinWalkVolume = 1f;
    protected override void Patrol()
    {
        if (!_isMoving || _isDead) return;

        _rb.velocity = new Vector2(_facingDirection * MoveSpeed, _rb.velocity.y);
        _animator?.SetBool("IsMoving", true);
        _animator?.SetBool("IsWalking", true);

        bool wallAhead = Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, StopDistance, GroundLayer);
        bool cliffAhead = !Physics2D.Raycast(CliffCheck.position, Vector2.down, 1.25f, GroundLayer);

        if (wallAhead || cliffAhead)
        {
            Flip();
        } else if (Random.value < RandomTurnChance) {
            Flip();
        }
    }
    public void UpdateVolume()
    {
        audioSource.volume = AmbientVolume * GoblinWalkVolume;
        DamageAudioSource.volume = SfxVolume * GoblinGetDamageVolume;
    }
    void Start()
    {
        WalkClip = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.GoblinWalk);
        DamageClip = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.GoblinGetDamage);
        // AudioSource[] sources = GetComponents<AudioSource>();
        // if (sources.Length >= 2)
        // {
        //     audioSource = sources[0];
        //     DamageAudioSource = sources[1];
        // }
        audioSource.volume = 1;
        audioSource.clip = WalkClip;
        DamageAudioSource.clip = DamageClip;
        audioSource.Play();
    }
    protected override IEnumerator GetHitted()
    {   
        Color originalColor = _renderer.color;

        _renderer.color = Color.grey;
        DamageAudioSource.PlayOneShot(DamageClip);
        yield return new WaitForSeconds(0.2f);

        _renderer.color = originalColor;
        
        StartCoroutine(HitKnockback());
    }
    private void SpawnCoins()
    {
        int coinsToSpawn = Random.Range(2, 5);
        for (int i = 0; i < coinsToSpawn; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 force = new Vector2(Random.Range(-12f, 12f), Random.Range(9f, 12f));
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }
    protected override void ChasePlayer()
    {
        if (_player == null || _isDead) return;

        float directionToPlayer = Mathf.Sign(_player.transform.position.x - transform.position.x);
        _facingDirection = (int)directionToPlayer;
        _rb.velocity = new Vector2(directionToPlayer * ChaseSpeed, _rb.velocity.y);
        _animator?.SetBool("IsMoving", true);
        _animator?.SetBool("IsWalking", false);

        if (Vector2.Distance(transform.position, _player.transform.position) > SightRange * 1.2f)
        {
            _isChasing = false;
        }
    }

    protected override void Die()
    {
        SpawnCoins();
        DamageAudioSource.PlayOneShot(DamageClip);
        _canTakeDamage = false;
        _renderer.color = Color.grey;
        _isDead = true;
        _animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = died;
        _rb.velocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Static;
        torch.localPosition = new Vector3(0.02048619f, -0.1183329f, -0.24f);
        torch.localRotation = Quaternion.Euler(0,0,270);
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(ReturnToGround());
    }

    private IEnumerator ReturnToGround()
    {
        float elapsedTime = 0f;
        float LerpTime = 0.1f;
        Vector2 currentHeight = transform.localPosition;
        Vector2 endHeight = new Vector2(transform.localPosition.x, DesiredHeight);
        while (elapsedTime < LerpTime)
        {
            elapsedTime += Time.deltaTime;

            transform.localPosition = Vector2.Lerp(currentHeight, endHeight, elapsedTime/LerpTime);
            
            yield return null;
        }
        transform.localPosition = endHeight;
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
