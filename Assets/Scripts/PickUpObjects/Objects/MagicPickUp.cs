using UnityEngine;

public class MagicPickUp : PickUpObject
{
    protected override void CollectItem()
    {
        // playerData.GrantAbility("HasMagic");
        PlayerDataSave.Instance.HasMagic = true;
        PlayerDataSave.Instance.MagicLevel = 1;
        Debug.Log("Magic collected!");
    }
}
