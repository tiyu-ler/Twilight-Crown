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
        // string sphereTypeName = sphereType.ToString();
        // switch (sphereTypeName)
        // {
        //     case "Sound": PlayerDataSave.Instance.CollectedSpheresSound = true;
        //     break;
        //     case "Light": PlayerDataSave.Instance.CollectedSpheresLight = true;
        //     break;
        // }
        // Debug.Log($"{sphereTypeName} orb collected!");
    }
}
