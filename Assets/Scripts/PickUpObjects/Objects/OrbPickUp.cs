using UnityEngine;

public class OrbPickUp : PickUpObject
{
    public enum OrbType
    {
        Sound,
        Light
    }
    [SerializeField] private OrbType sphereType;

    protected override void CollectItem()
    {
        string sphereTypeName = sphereType.ToString();
        playerData.CollectSphere(sphereTypeName);
        Debug.Log($"{sphereTypeName} orb collected!");
    }
}
