using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public abstract class MonsterScript : MonoBehaviour
{

    [Header("Stats and Settings")]
    public float MaxHealth;
    public float MoveSpeed;
    public float ChaseSpeed;
    public float StopDistance;
    public int Damage;
    public static List<MonsterScript> ActiveMonsters = new List<MonsterScript>();
    

    [Header("Monster AI")]
    public float SightRange;
    public float RandomTurnChance;
    public Transform WallCheck;
    public Transform CliffCheck;
    public LayerMask GroundLayer;
    public LayerMask PlayerLayer;


    protected float _currentHealth;
    protected bool _isDead = false;
    protected bool _isChasing = false;
    protected bool _isMoving = true;
    protected bool _canTakeDamage = true;
    protected Rigidbody2D _rb;
    protected SpriteRenderer _renderer;
    protected Animator _animator;
    protected GameObject _player;
    protected int _facingDirection = 1; // 1 - right, -1 - left
    protected bool _markedToDie = false;
    // protected static int _lastAttackFrame = -1;
    protected static bool _hasLaunchedPlayer = false;
    protected Coroutine _airLaunch;
    protected virtual void Awake()
    {
        _player = FindObjectOfType<PlayerMovement>().gameObject;
        ActiveMonsters.Add(this);
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _currentHealth = MaxHealth;
    }

    protected virtual void OnDestroy()
    {
        ActiveMonsters.Remove(this);
    }

    public void StopEnemy()
    {
        _isMoving = false;
        _isChasing = false;
        _rb.velocity = Vector2.zero;
        _animator.StopPlayback();
        _animator.speed = 0;
    }
    public void StartEnemy()
    {
        _isMoving = true;
        _isChasing = true;
        _currentHealth = MaxHealth;
        _rb.velocity = Vector2.zero;
        _animator.StartPlayback();
        _animator.speed = 1;
    }

    protected virtual void Update()
    {
        if (_isDead) return;

        if (_rb.velocity.y < 0.1f && _rb.velocity.y > -0.1f && _markedToDie) Die();

        Patrol();
        CheckForPlayer();
    }

    protected virtual void LateUpdate()
    {
        if (_isDead) return;

        CheckForPlayer();
    }

    protected virtual void Patrol()
    {
        if (!_isMoving) return;

        _rb.velocity = new Vector2(_facingDirection * MoveSpeed, _rb.velocity.y);
        _animator?.SetBool("IsMoving", true);

        bool wallAhead = Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, StopDistance, GroundLayer);
        bool cliffAhead = !Physics2D.Raycast(CliffCheck.position, Vector2.down, 1.25f, GroundLayer);

        if (wallAhead || cliffAhead)
        {
            Flip();
        } else if (Random.value < RandomTurnChance) {
            Flip();
        }
    }


    protected virtual void CheckForPlayer() 
    {
        // if (Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, SightRange, GroundLayer)) return;
        RaycastHit2D hit = Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, SightRange, PlayerLayer);
        RaycastHit2D hitReversed = Physics2D.Raycast(WallCheck.position, Vector2.left * _facingDirection, SightRange, PlayerLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            _player = hit.collider.gameObject;
            _isChasing = true;
        }
        if (hitReversed.collider != null && hitReversed.collider.CompareTag("Player"))
        {
            Flip();
        }
    } // Для других монстров, кроме носорога, добавить override и сделать просмотр в 2 стороны и делать поворот в сторону игрока


    protected virtual void ChasePlayer()
    {
        if (_player == null) return;

        float directionToPlayer = Mathf.Sign(_player.transform.position.x - transform.position.x);
        _facingDirection = (int)directionToPlayer;
        _rb.velocity = new Vector2(directionToPlayer * ChaseSpeed, _rb.velocity.y);
        _animator?.SetBool("IsMoving", true);

        if (Vector2.Distance(transform.position, _player.transform.position) > SightRange * 1.2f)
        {
            _isChasing = false;
        }
    }


    protected void Flip()
    {
        if (_rb.velocity.y > 0.1f && _rb.velocity.y < -0.1f) return;
        _facingDirection *= -1;
        transform.localScale = new Vector3(_facingDirection*6, 6, 0);
    }


    public virtual void TakeDamage(float damage, string attackDirection)
    {
        if (_isDead || !_canTakeDamage) return;
        
        if (attackDirection == "bottom" && !_hasLaunchedPlayer)
        {
            // _hasLaunchedPlayer = true;
            // _player.GetComponent<PlayerMovement>().RigidBody.AddForce(Vector2.up * 30f, ForceMode2D.Impulse);
            // StartCoroutine(ResetLaunchFlag());
            if (_airLaunch == null)
            {
                _airLaunch = StartCoroutine(LaunchInTheAir());
            }
        }

        _currentHealth -= damage;
        _canTakeDamage = false;
        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(GetHitted());
        }
    }
    private IEnumerator LaunchInTheAir()
    {
        yield return new WaitForSeconds(0.05f);
        _hasLaunchedPlayer = true;
        _player.GetComponent<PlayerMovement>().RigidBody.AddForce(Vector2.up * 30f, ForceMode2D.Impulse);
        StartCoroutine(ResetLaunchFlag());
    }
    private IEnumerator ResetLaunchFlag()
    {
        yield return new WaitForSeconds(0.1f); // Small delay before allowing another launch
        _hasLaunchedPlayer = false;
    }
    protected virtual IEnumerator GetHitted()
    {   
        Color originalColor = _renderer.color;

        _renderer.color = Color.grey;

        yield return new WaitForSeconds(0.2f);

        _renderer.color = originalColor;
        
        StartCoroutine(HitKnockback());
    }

    protected virtual IEnumerator HitKnockback()
    {
        float knockbackForce = 4f;
        float knockbackDuration = 0.05f;

        Vector2 knockbackDirection = (transform.position - _player.transform.position).normalized;
        knockbackDirection.y = 0.5f;

        _rb.velocity = Vector2.zero;
        _rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        _isMoving = false;

        yield return new WaitForSeconds(knockbackDuration);

        // Re-enable movement
        _canTakeDamage = true;
        _isMoving = true;
    }


    protected virtual void Die()
    {
        _canTakeDamage = false;
        _renderer.color = Color.grey;
        _isDead = true;
        _animator.Play("Die");
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        // Destroy(gameObject, 2f);
    }

    
    // protected void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.blue;
    //     Vector3 pos = new Vector3(CliffCheck.transform.position.x, CliffCheck.transform.position.y, CliffCheck.transform.position.z);
    //     Gizmos.DrawRay(pos, Vector2.down * 1.25f);
    //     Gizmos.color = Color.red;
    //     Vector3 newpos = new Vector3(WallCheck.transform.position.x, WallCheck.transform.position.y, WallCheck.transform.position.z);
    //     Gizmos.DrawRay(newpos, Vector2.right);
    // }
}
