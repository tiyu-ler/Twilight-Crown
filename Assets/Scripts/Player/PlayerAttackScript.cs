using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float AttackCooldown = 0.2f;
    
    [Header("Hitboxes")]
    public GameObject Hitbox_Right;
    public GameObject Hitbox_Up;
    public GameObject Hitbox_Down;
    public GameObject Hitbox_Air;

    [Header("Animators")]
    public Animator UpperBodyAnimator;

    public bool _canAttack = false;
    private bool _doubleAttack = false;
    private Coroutine _hideSwordCoroutine;
    private PlayerMovement playerMovement;
    private Rigidbody2D _rb;

    void Start()
    {
        _canAttack = PlayerDataSave.Instance.HasSword;
        playerMovement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();

        DisableAllHitboxes(); // Ensure hitboxes are off at start
    }

    private void Update()
    {
        if (_canAttack && Input.GetMouseButtonDown(0) && !playerMovement.IsDashing)
        {
            if (Input.GetKey(KeyCode.W)) // Up attack
            {
                PerformAttack(Hitbox_Up, "U_UpAttack_1", "U_UpAttack_2", false);
            }
            else if (!playerMovement.IsGrounded() && Input.GetKey(KeyCode.S)) // Down attack in air
            {
                PerformAttack(Hitbox_Down, "U_Attack_Down1", "U_Attack_Down2", true);
            }
            else if (playerMovement.IsGrounded() && Mathf.Abs(_rb.velocity.x) > 0.1f) // Ground attack while moving
            {
                PerformAttack(Hitbox_Right, "U_Attack_1", "U_Attack_2", false);
            }
            else
            {
                PerformAttack(Hitbox_Air, "U_AttackAir1", "U_AttackAir2", false); // Air attack or idle attack
            }
        }
    }

    private void PerformAttack(GameObject hitbox, string animationName, string alternativeName, bool isDownSlash)
    {
        string currentAnimation;
        _canAttack = false;

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

        // Enable hitbox for the attack
        StartCoroutine(EnableHitboxTemporarily(hitbox));
        int i = Random.Range(0, 5);
        SoundManager.SoundID swingSound = SoundManager.SoundID.SwordSwing1 + i;
        SoundManager.Instance.PlaySound(swingSound);
        // Wait for the animation to finish
        StartCoroutine(WaitForAttackAnimation(currentAnimation));

        if (_hideSwordCoroutine != null)
            StopCoroutine(_hideSwordCoroutine);

        _hideSwordCoroutine = StartCoroutine(HideSword(isDownSlash));
    }

    private IEnumerator HideSword(bool isDownSlash)
    {
        yield return new WaitForSeconds(0.5f);
        UpperBodyAnimator.SetBool("DiscardAttack", true);
        int i = Random.Range(0, 5);
        SoundManager.SoundID swingSound = SoundManager.SoundID.SwordSwing1 + i;
        SoundManager.Instance.PlaySound(swingSound);
        if (_doubleAttack && !isDownSlash)
            UpperBodyAnimator.Play("U_HideSword_1");
        else
            UpperBodyAnimator.Play("U_HideSword_2"); // нижний

        _doubleAttack = false;
    }

    private IEnumerator EnableHitboxTemporarily(GameObject hitbox)
    {
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        hitbox.SetActive(false);
    }

    private IEnumerator WaitForAttackAnimation(string attackAnimation)
    {
        yield return new WaitUntil(() => UpperBodyAnimator.GetCurrentAnimatorStateInfo(0).IsName(attackAnimation));
        yield return new WaitUntil(() => UpperBodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        _canAttack = true;
    }

    private void DisableAllHitboxes()
    {
        Hitbox_Right.SetActive(false);
        Hitbox_Up.SetActive(false);
        Hitbox_Down.SetActive(false);
        Hitbox_Air.SetActive(false);
    }
}