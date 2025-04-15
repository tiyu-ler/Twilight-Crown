using System.Collections;
using UnityEngine;
public class DashPickUp : PickUpObject
{
    public DoorScript doorScript;
    protected override void CollectItem()
    {
        PlayerDataSave.Instance.HasDash = true;
    }

    protected override void Collect()
    {
        if (pickupAnimationName != "")
        {
            animator.Play(pickupAnimationName);
            SoundManager.Instance.PlaySound(SoundManager.SoundID.DashPickUp, worldPos: transform.position, volumeUpdate: 0.4f);
            LowerBody.SetActive(false);
            UpperBody.SetActive(false);
            playerMovement.StopSound = true;
        }

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

            UpperBody.GetComponent<Animator>().Play("U_Idle");
            LowerBody.GetComponent<Animator>().Play("L_Idle");
        }  

        playerAttack.enabled = true;
        playerMovement.FlipCharacter(true, 1);
        playerMovement.CanMove = true;
        playerMovement.CanDash = true;
        playerMovement.StopSound = false;
        
        if (cameraFollowObject.transform.rotation.y != Player.transform.rotation.y)
        {
            cameraFollowObject.CallTurn();
        }
        gameManager.SaveGame(PlayerDataSave.Instance.saveID);
        markerTextPopUp.DisableMarkUp();
        gameManager.UpdateObjectsBySaveInfo();
        doorScript.DoorInteractor(false, true);
        Destroy(gameObject);
    }
}
