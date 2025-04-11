using System.Collections;
using UnityEngine;
public class DashPickUp : PickUpObject
{
    protected override void CollectItem()
    {
        PlayerDataSave.Instance.HasDash = true;
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
        
        if (cameraFollowObject.transform.rotation.y != Player.transform.rotation.y)
        {
            cameraFollowObject.CallTurn();
        }
        gameManager.SaveGame(PlayerDataSave.Instance.saveID);
        markerTextPopUp.DisableMarkUp();
        Destroy(gameObject);
    }
}
