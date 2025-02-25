using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float AttackCooldown = 0.2f; // Time between attacks
    public float AttackRange = 1.5f; // Range of the attack
    public LayerMask EnemyLayer; // What the attack can hit
    public Transform AttackPoint; // Position of attack origin

    private float _attackTimer = 0;
    private bool _canAttack = true;
    private bool _doubleAttack = false;
    private Coroutine _hideSwordCoroutine;

    public Animator UpperBodyAnimator;
    public Animator LowerBodyAnimator;
    private PlayerMovement playerMovement;
    private Rigidbody2D _rb;
    void Start()
    {
        Time.timeScale = 0.25f; 
        playerMovement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        _attackTimer -= Time.deltaTime;

        if (_canAttack && Input.GetMouseButtonDown(0))
        {
            Debug.Log(playerMovement.IsGrounded());
            if (Input.GetKey(KeyCode.W) && _canAttack) //вверх
            {
                PerformAttack(Vector2.up, "U_UpAttack_1", "U_UpAttack_2", false);
            }
            else if (!playerMovement.IsGrounded() && Input.GetKey(KeyCode.S) && _canAttack) // вниз в воздухе
            {
                    PerformAttack(Vector2.down, "U_Attack_Down1", "U_Attack_Down2", true);
            }
            // else if (playerMovement.IsGrounded() && Mathf.Abs(_rb.velocity.x) < 0.01f && _canAttack) // на земле, если не двигаеться
            else if (playerMovement.IsGrounded() && _canAttack) // на земле, если не двигаеться
            // else if (Mathf.Abs(_rb.velocity.x) < 0.01f && _canAttack) // если не двигаеться
            {
                PerformAttack(Vector2.down, "U_Attack_1", "U_Attack_2", false);
            }
            else
            {
                PerformAttack(Vector2.right, "U_AttackAir1", "U_AttackAir2", false); //
            }
        }
    }

    private void PerformAttack(Vector2 attackDirection, string animationName, string alternativeName, bool isDownSlash)
    {
        string currentAnimation;
        _canAttack = false;
        _attackTimer = AttackCooldown;
    
        // UpperBodyAnimator.Play(animationName);
        if (!_doubleAttack)
        {
            UpperBodyAnimator.Play(animationName);
            _doubleAttack = true;
            currentAnimation = animationName;
        }
        else
        {
            UpperBodyAnimator.Play(alternativeName);
            _doubleAttack = false;
            currentAnimation = alternativeName;
        }

        DetectHits(attackDirection);

        if (_hideSwordCoroutine != null)
            StopCoroutine(_hideSwordCoroutine);

        _hideSwordCoroutine = StartCoroutine(HideSword(isDownSlash));
        
        StartCoroutine(WaitForAttackAnimation(currentAnimation));
    }

    private IEnumerator HideSword(bool isDownSlash)
    {
        yield return new WaitForSeconds(0.42f);
        UpperBodyAnimator.SetBool("DiscardAttack", true);

        if (_doubleAttack && !isDownSlash)
            UpperBodyAnimator.Play("U_HideSword_1");
        else
            UpperBodyAnimator.Play("U_HideSword_2"); // for ground Attack

        _doubleAttack = false; // Reset attack order after hiding the sword
    }

    private IEnumerator WaitForAttackAnimation(string attackAnimation)
    {
        yield return new WaitUntil(() => UpperBodyAnimator.GetCurrentAnimatorStateInfo(0).IsName(attackAnimation));
        yield return new WaitUntil(() => UpperBodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        _canAttack = true;
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

    private void OnDrawGizmos()
    {
        if (AttackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(AttackPoint.position, AttackPoint.position + Vector3.right * AttackRange);
        }
    }
}
