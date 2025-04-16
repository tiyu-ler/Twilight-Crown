using System.Collections;
using UnityEngine;

public class ClimbPickUp : PickUpObject
{
    public Sprite EmptyBag;
    public GameObject Light;
    public Vector2 AnimationEndLocation;
    protected override void CollectItem()
    {
        PlayerDataSave.Instance.HasWallClimb = true;
        Debug.Log("WallClimb collected!");
    }
    protected override void Collect()
    {
        if (pickupAnimationName != "")
        {
            animator.Play(pickupAnimationName);
            SoundManager.Instance.PlaySound(SoundManager.SoundID.WallJumpPickUp1, worldPos: transform.position, volumeUpdate: 0.45f);
            StartCoroutine(PlayAfterDelay());
            LowerBody.SetActive(false);
            UpperBody.SetActive(false);
            playerMovement.StopSound = true;
        }
        Player.transform.position = AnimationEndLocation;
        StartCoroutine(DestroyAfterAnimation());
        CollectItem();
    }

    private IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(0.6f);
        SoundManager.Instance.PlaySound(SoundManager.SoundID.WallJumpPickUp1, worldPos: transform.position, volumeUpdate: 0.45f);
    }
    protected override IEnumerator DestroyAfterAnimation()
    {
        if (pickupAnimationName != "")
        {
            yield return new WaitForSeconds(GetAnimationLength());

            LowerBody.SetActive(true);
            UpperBody.SetActive(true);

            UpperBody.GetComponent<Animator>().Play("U_Idle");
            LowerBody.GetComponent<Animator>().Play("L_Idle");
        }
        Destroy(Light);
        playerAttack.enabled = true;
        // Debug.Log("Player rotation " + Player.transform.rotation.y);
        playerMovement.FlipCharacter(true, 1);
        playerMovement.CanMove = true;
        playerMovement.CanWallClimb = true;
        playerMovement.StopSound = false;
        // Debug.Log(cameraFollowObject.transform.rotation.y != Player.transform.rotation.y);
        
        if (cameraFollowObject.transform.rotation.y != Player.transform.rotation.y)
        {
            cameraFollowObject.CallTurn();
        }
        animator.enabled = false;
        gameManager.SaveGame(PlayerDataSave.Instance.saveID);
        GetComponent<SpriteRenderer>().sprite = EmptyBag;
        gameManager.UpdateObjectsBySaveInfo();
    }

    public void SetEmptyAnimation()
    {
        GetComponent<SpriteRenderer>().sprite = EmptyBag;
    }
}
