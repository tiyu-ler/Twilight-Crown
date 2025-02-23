using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float AttackCooldown = 0.2f; // Time between attacks
    public float AttackRange = 1.5f; // Range of the attack
    public LayerMask EnemyLayer; // What the attack can hit
    public Transform AttackPoint; // Position of attack origin

    private float _attackTimer = 0;
    private bool _isAttacking = false;
    public Animator UpperBodyAnimator;
    public Animator LowerBodyAnimator;
    private int _attackCount = 0;
    private void Update()
    {
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0 && Input.GetMouseButtonDown(0)) // 0 - left, 1 - right, 2 - middle
        {
            Debug.Log("W");
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        _isAttacking = true;
        _attackTimer = AttackCooldown;

        Vector2 attackDirection = Vector2.right; // Default attack direction

        if (Input.GetKey(KeyCode.W)) // Upward attack
        {
            attackDirection = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.S) && !IsGrounded()) // Downward attack (only in air)
        {
            attackDirection = Vector2.down;
        }
        else
        {
            _attackCount += 1;
            // _playerAnimator.SetInteger("AttackCounter", _attackCount);
            // _playerAnimator.SetBool("Attacking", true);
        }

        // _animator.SetTrigger(animationTrigger);
        DetectHits(attackDirection);
    }

    private void DetectHits(Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(AttackPoint.position, direction, AttackRange, EnemyLayer);
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.collider.name);
                // Implement enemy damage logic here
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
    }

    private void OnDrawGizmos()
    {
        // if (AttackPoint != null)
        // {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(AttackPoint.position, AttackPoint.position + Vector3.right * AttackRange);
        // }
    }
}
