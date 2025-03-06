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
        moveSpeed = playerMovement.MoveSpeed;
        playerAttack.enabled = false;
    }
    protected override void CollectItem()
    {
        PlayerDataSave.Instance.HasSword = true;
        PlayerDataSave.Instance.SwordLevel = 1;
        playerAttack._canAttack = true;
        // Debug.Log("Sword collected!");
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
        // float elapsedTime = 0f;
        // float LerpTime = 0.5f;
        // while (elapsedTime < LerpTime)
        // {
        //     elapsedTime += Time.deltaTime;

        //     BlackBorderTop.transform.localPosition = new Vector3(0, Mathf.Lerp(440, 640, elapsedTime/LerpTime), 0);
        //     BlackBorderBottom.transform.localPosition = new Vector3(0, Mathf.Lerp(-440, -640, elapsedTime/LerpTime), 0);
            
        //     yield return null;
        // }
        // cameraManager.CutsceneCamera(false);
        playerAttack.enabled = true;
        playerMovement.FlipCharacter(true, 1);
        playerMovement.CanMove = true;
        // BlackBorderTop.SetActive(false);
        // BlackBorderBottom.SetActive(false);
        // Debug.Log(FindAnyObjectByType<CameraFollowObject>().transform.rotation.y != Player.transform.rotation.y);
        if (FindAnyObjectByType<CameraFollowObject>().transform.rotation.y != Player.transform.rotation.y)
        {
            Debug.Log("CALLTURN");
            playerMovement.IsFacingRight = !playerMovement.IsFacingRight;
            FindAnyObjectByType<CameraFollowObject>().CallTurn();
        }
        Destroy(gameObject);
    }
}
