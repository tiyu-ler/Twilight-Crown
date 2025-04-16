using System.Collections;
using UnityEngine;

public class RhinoMonster : MonsterScript
{
    public GameObject coinPrefab;
    [Header("Rolling Settings")]
    public float RollSpeed = 10f;
    public float MinRollDuration = 1f;
    public float MaxRollDuration = 3f;
    
    [Header("Colliders Settings")]
    public Collider2D DefaultCollider;
    public Collider2D RollCollider;
    public Sprite died;
    private bool _isRolling = false;
    private bool _isRollingAnimationFinished = false;
    private float _rollTimer = 0f;
    private AudioClip WalkClip, RollClip, CurlClip, HitWallClip, DamageClip;
    public AudioSource audioSource;
    public AudioSource additionalAdioSource;
    public AudioSource DamageAudioSource;
    private float AmbientVolume, SfxVolume;
    private Vector3 _startPosition;
    private Color _startColor;
    private const float RhinoCurlVolume = 0.1f, RhinoHitWallVolume = 0.9f, RhinoRollingVolume = 0.03f, RhinoWalkVolume = 0.04f;
    private const float RhinoGetDamageVolume = 0.075f;
    protected override void Update()
    {
        if (IsDead) return;

        if (_rb.velocity.y < 0.1f && _rb.velocity.y > -0.1f && _markedToDie) Die();

        if (_isRolling)
        {
            if (_isRollingAnimationFinished) 
                Roll();
        }
        else if (_isChasing)
        {
            StartRolling();
        }
        else
        {
            Patrol();
            CheckForPlayer();
        }
    }
    public void RessurectMonster()
    {
        _markedToDie = false;
        IsDead = false;
        _isRolling = false;
        _isChasing = false;
        transform.localPosition = _startPosition;
        audioSource.enabled = true;
        additionalAdioSource.enabled = true;
        audioSource.Play();
        additionalAdioSource.Play();
        DefaultCollider.isTrigger = false;
        DefaultCollider.enabled = true;
        RollCollider.enabled = false;
        _canTakeDamage = true;
        _renderer.color = _startColor; 
        _animator.enabled = true;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<CapsuleCollider2D>().enabled = true;
        _currentHealth = MaxHealth;
        _animator.SetBool("IsMoving", true);
        StopRolling();
    }

    protected override void CheckForPlayer() 
    {
        // if (Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, SightRange, GroundLayer)) return;

        RaycastHit2D hit = Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, SightRange, PlayerLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            _player = hit.collider.gameObject;
            _isChasing = true;
        }
    }
    void Start()
    {
        WalkClip = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.RhinoWalk);
        RollClip = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.RhinoRolling);
        CurlClip = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.RhinoCurl);
        HitWallClip = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.RhinoHitWall);
        DamageClip = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.RhinoGetDamage);
        _startPosition = transform.localPosition;
        _startColor = _renderer.color;
    }
    public void UpdateVolume(float ambient, float sfx)
    {
        AmbientVolume = ambient;
        SfxVolume = sfx;
        DamageAudioSource.volume = SfxVolume * RhinoGetDamageVolume;
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
    protected override void Patrol()
    {
        if (!_isMoving || _isRolling) return;

        if (audioSource.clip != WalkClip || !audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = WalkClip;
            audioSource.volume = AmbientVolume * RhinoWalkVolume;
            audioSource.Play();
        }
        
        _rb.velocity = new Vector2(_facingDirection * MoveSpeed, _rb.velocity.y);
        _animator.SetBool("IsMoving", true);

        bool wallAhead = Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, StopDistance, GroundLayer);
        bool cliffAhead = !Physics2D.Raycast(CliffCheck.position, Vector2.down, 1.25f, GroundLayer);

        if (wallAhead || cliffAhead)
        {
            Flip();
        }

        if (Random.value < RandomTurnChance)
        {
            Flip();
        }
    }


    private void StartRolling()
    {
        if (_isRolling) return;

        _isRolling = true;
        _isRollingAnimationFinished = false;
        _rollTimer = Random.Range(MinRollDuration, MaxRollDuration);

        _rb.velocity = Vector2.zero;
        _animator.SetBool("CanMove", false);
        _animator.Play("StartRoll");
        RollCollider.enabled = true;
        DefaultCollider.enabled = false;
        StartCoroutine(WaitForStartRollAnimation());
    }

    public override void TakeDamage(float damage, string attackDirection)
    {
        if (IsDead || !_canTakeDamage) return;
        switch(attackDirection)
        {
            case "bottom": _player.GetComponent<PlayerMovement>().RigidBody.AddForce(Vector2.up * 30f, ForceMode2D.Impulse);
                break;
        }
        _currentHealth -= damage;
        _canTakeDamage = false;
        // Debug.Log(_currentHealth);
        if (_currentHealth <= 0)
        {
            // Die();
            _markedToDie = true;
        }
        else
        {
            StartCoroutine(GetHitted());
        }
    }

    private IEnumerator WaitForStartRollAnimation()
    {
        audioSource.Stop();
        additionalAdioSource.pitch = 1;
        additionalAdioSource.volume = AmbientVolume * RhinoCurlVolume;
        additionalAdioSource.PlayOneShot(CurlClip);
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName("StartRoll"));
        audioSource.Stop();
        audioSource.clip = RollClip;
        audioSource.volume = AmbientVolume * RhinoRollingVolume;
        audioSource.Play();
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        
        _isRollingAnimationFinished = true;
        _animator.Play("Roll");
    }


    private void Roll()
    {
        _rb.velocity = new Vector2(_facingDirection * RollSpeed, _rb.velocity.y);
        _rollTimer -= Time.deltaTime;

        bool wallAhead = Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, StopDistance, GroundLayer);
        bool cliffAhead = !Physics2D.Raycast(CliffCheck.position, Vector2.down, 1f, GroundLayer);

        if (_rollTimer <= 0 || wallAhead || cliffAhead)
        {
            additionalAdioSource.pitch = 0.5f;
            additionalAdioSource.volume = AmbientVolume * RhinoHitWallVolume;
            audioSource.PlayOneShot(HitWallClip);
            StopRolling();
        }
    }


    private void StopRolling()
    {
        _isRollingAnimationFinished = false;
        _rb.velocity = Vector2.zero;
        _animator.Play("UnRoll");
        StartCoroutine(WaitForUnRollAnimation());
    }

    protected override IEnumerator HitKnockback()
    {
        if (_isRolling)
        {
            DefaultCollider.isTrigger = false;
            RollCollider.isTrigger = false;

            float knockbackForce = 4f;
            float knockbackDuration = 0.05f;

            Vector2 knockbackDirection = (transform.position - _player.transform.position).normalized;
            knockbackDirection.y = 0.5f;

            _rb.velocity = Vector2.zero;
            _rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            _isMoving = false;

            yield return new WaitForSeconds(knockbackDuration);

            DefaultCollider.isTrigger = true;
            RollCollider.isTrigger = true;

            // Re-enable movement
        }
        _isMoving = true;
        _canTakeDamage = true;
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
    protected override void Die()
    {
        SpawnCoins();
        audioSource.Stop();
        additionalAdioSource.Stop();
        audioSource.enabled = false;
        additionalAdioSource.enabled = false;
        DamageAudioSource.PlayOneShot(DamageClip);
        DefaultCollider.isTrigger = false;
        DefaultCollider.enabled = true;
        RollCollider.enabled = false;
        _canTakeDamage = false;
        _renderer.color = Color.grey;
        IsDead = true;
        _animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = died;
        _rb.velocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        audioSource.PlayOneShot(DamageClip);
    }
    private IEnumerator WaitForUnRollAnimation()
    {
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName("UnRoll"));
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        _isRolling = false;

        _isChasing = false;
        _animator.SetBool("CanMove", true);
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
