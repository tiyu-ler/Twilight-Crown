using System.Collections;
using UnityEngine;

public abstract class MonsterScript : MonoBehaviour
{

    [Header("Stats")]
    public float MaxHealth;
    public float MoveSpeed;
    public float ChaseSpeed;
    public float StopDistance;
    public int Damage;


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
    protected Rigidbody2D _rb;
    protected SpriteRenderer _renderer;
    protected Animator _animator;
    protected Transform _player;
    protected int _facingDirection = 1; // 1 - right, -1 - left


    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _currentHealth = MaxHealth;
    }


    protected virtual void Update()
    {
        if (_isDead) return;

        if (_isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            CheckForPlayer();
        }
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
        if (Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, SightRange, GroundLayer)) return;

        RaycastHit2D hit = Physics2D.Raycast(WallCheck.position, Vector2.right * _facingDirection, SightRange, PlayerLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            _player = hit.collider.transform;
            _isChasing = true;
        }
    } // Для других монстров, кроме носорога, добавить override и сделать просмотр в 2 стороны и делать поворот в сторону игрока


    protected virtual void ChasePlayer()
    {
        if (_player == null) return;

        float directionToPlayer = Mathf.Sign(_player.position.x - transform.position.x);
        _facingDirection = (int)directionToPlayer;
        _rb.velocity = new Vector2(directionToPlayer * ChaseSpeed, _rb.velocity.y);
        _animator?.SetBool("IsMoving", true);

        if (Vector2.Distance(transform.position, _player.position) > SightRange * 1.2f)
        {
            _isChasing = false;
        }
    }


    protected void Flip()
    {
        _facingDirection *= -1;
        transform.localScale = new Vector3(_facingDirection*6, 6, 0);
    }


    public virtual void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;
        
        StartCoroutine(GetHitted());

        if (_currentHealth <= 0)
        {
            Die();
        }
    }


    private IEnumerator GetHitted()
    {   
        Color originalColor = _renderer.color;

        _renderer.color = Color.white;

        yield return new WaitForSeconds(0.2f);

        _renderer.color = originalColor;
    }


    protected virtual void Die()
    {
        _isDead = true;
        _animator?.SetTrigger("Die");
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }


    protected abstract void Attack();

}
