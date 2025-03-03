using UnityEngine;

public class MagicPickUp : PickUpObject
{
    protected override void CollectItem()
    {
        playerData.GrantAbility("HasMagic");
        Debug.Log("Magic collected!");
    }
}
