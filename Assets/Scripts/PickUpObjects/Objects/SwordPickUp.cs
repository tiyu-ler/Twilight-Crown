using System.Collections;
using UnityEngine;

public class SwordPickUp : PickUpObject
{
    protected override void Start()
    {
        _lowerBodyAnimator = LowerBody.GetComponent<Animator>();
        _upperBodyAnimator = UpperBody.GetComponent<Animator>();
        animator = GetComponent<Animator>();
        collider2d = GetComponent<BoxCollider2D>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerAttack = FindObjectOfType<PlayerAttack>();
        playerData = FindObjectOfType<PlayerData>();
        moveSpeed = playerMovement.MoveSpeed;
        playerAttack.enabled = false;
    }
    protected override void CollectItem()
    {
        playerData.GrantAbility("HasSword");
        Debug.Log("Sword collected!");
    }

    protected override void Collect()
    {
        if (pickupAnimationName != "")
        {
            Destroy(SwordCase);
            animator.Play(pickupAnimationName);
            LowerBody.SetActive(false);
            UpperBody.SetActive(false);
        }
        Player.transform.position = EndPosition;
        StartCoroutine(DestroyAfterAnimation());
        CollectItem();
    }

    protected override IEnumerator DestroyAfterAnimation()
    {
        if (pickupAnimationName != "")
        {
            yield return new WaitForSeconds(GetAnimationLength());
            LowerBody.SetActive(true);
            UpperBody.SetActive(true);

            UpperBody.GetComponent<Animator>().SetBool("HasSword", true);

            UpperBody.GetComponent<Animator>().Play("U_Idle");
            LowerBody.GetComponent<Animator>().Play("L_Idle");
        }
        Player.transform.position = EndPosition;
        // playerMovement.enabled = true;
        playerAttack.enabled = true;
        playerMovement.FlipCharacter(true, 1);
        // playerMovement.FlipCharacter(true, 1);
        playerMovement.CanMove = true;
        Destroy(gameObject);
    }
}
