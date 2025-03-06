using System.Collections;
using UnityEngine;

public class RhinoMonster : MonsterScript
{
    [Header("Rolling Settings")]
    public float RollSpeed = 10f;
    public float MinRollDuration = 1f;
    public float MaxRollDuration = 3f;
    
    [Header("Colliders Settings")]
    public Collider2D DefaultCollider;
    public Collider2D RollCollider;

    private bool _isRolling = false;
    private bool _isRollingAnimationFinished = false;
    private float _rollTimer = 0f;
    protected override void Update()
    {
        if (_isDead) return;

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


    protected override void Patrol()
    {
        if (!_isMoving || _isRolling) return;

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
        if (_isDead || !_canTakeDamage) return;
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
            Die();
        }
        else
        {
            StartCoroutine(GetHitted());
        }
    }

    private IEnumerator WaitForStartRollAnimation()
    {
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName("StartRoll"));
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
    protected override void Die()
    {
        DefaultCollider.isTrigger = false;
        DefaultCollider.enabled = true;
        RollCollider.enabled = false;
        _canTakeDamage = false;
        _renderer.color = Color.grey;
        _isDead = true;
        _animator.Play("Die");
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
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
        if (_isRolling && collision.CompareTag("Player"))
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
