using System.Collections;
using UnityEngine;

public class SwordPickUp : PickUpObject
{
    public GameObject SwordCase;
    protected override void Start()
    {
        _lowerBodyAnimator = LowerBody.GetComponent<Animator>();
        _upperBodyAnimator = UpperBody.GetComponent<Animator>();
        animator = GetComponent<Animator>();
        collider2d = GetComponent<BoxCollider2D>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerAttack = FindObjectOfType<PlayerAttack>();
        moveSpeed = playerMovement.MoveSpeed;
        playerAttack.enabled = false;
    }
    protected override void CollectItem()
    {
        PlayerDataSave.Instance.HasSword = true;
        PlayerDataSave.Instance.SwordLevel = 0;
        playerAttack._canAttack = true;
    }

    protected override void Collect()
    {
        if (pickupAnimationName != "")
        {
            Destroy(SwordCase);
            animator.Play(pickupAnimationName);
            // SoundManager.Instance.PlaySound(SoundManager.SoundID.SwordPickUp, worldPos: transform.position, volumeUpdate: 0.4f);
            LowerBody.SetActive(false);
            UpperBody.SetActive(false);
            playerMovement.StopSound = true;
        }
        Player.transform.position = EndPosition;
        StartCoroutine(DestroyAfterAnimation());
        CollectItem();
    }

    protected override IEnumerator DestroyAfterAnimation()
    {
        if (pickupAnimationName != "")
        {
            yield return new WaitForSeconds(GetAnimationLength()*0.5f);
            SoundManager.Instance.PlaySound(SoundManager.SoundID.SwordPickUp, worldPos: transform.position, volumeUpdate: 0.4f);
            yield return new WaitForSeconds(GetAnimationLength()*0.5f);
            animator.enabled = false;
            LowerBody.SetActive(true);
            UpperBody.SetActive(true);

            UpperBody.GetComponent<Animator>().SetBool("HasSword", true);

            UpperBody.GetComponent<Animator>().Play("U_Idle");
            LowerBody.GetComponent<Animator>().Play("L_Idle");
        }
        Player.transform.position = EndPosition;

        playerAttack.enabled = true;
        playerMovement.FlipCharacter(true, 1);
        playerMovement.CanMove = true;
        playerMovement.StopSound = false;
        if (FindAnyObjectByType<CameraFollowObject>().transform.rotation.y != Player.transform.rotation.y)
        {
            playerMovement.IsFacingRight = !playerMovement.IsFacingRight;
            FindAnyObjectByType<CameraFollowObject>().CallTurn();
        }
        gameManager.SaveGame(PlayerDataSave.Instance.saveID);
        gameManager.UpdateObjectsBySaveInfo();
        Destroy(gameObject);
    }
}
