using UnityEngine;

public class GoblinMonster : MonsterScript
{
    public Sprite died;
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
        _canTakeDamage = false;
        _renderer.color = Color.grey;
        _isDead = true;
        _animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = died;
        _rb.velocity = Vector2.zero;
        // _rb.isKinematic = true;
        _rb.bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
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
