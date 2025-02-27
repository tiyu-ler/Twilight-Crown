using UnityEngine;


public class PlayerData : MonoBehaviour
{
    public bool HasSword { get; private set; }
    public int SwordLevel { get; private set; }
    public bool HasWallClimb { get; private set; }
    public bool HasDash { get; private set; }
    public bool HasMagic { get; private set; }
    public int MagicLevel { get; private set; }
    public bool CollectedSpheresSound { get; private set; }
    public bool CollectedSpheresLight { get; private set; }
    public int Money { get; private set; }

    private void Start()
    {
        Load();
    }

    public void GrantAbility(string ability)
    {
        switch (ability)
        {
            case "HasSword":
                HasSword = true;
                break;
            case "SwordLevel":
                SwordLevel += 1;
                break;
            case "WallClimb":
                HasWallClimb = true;
                break;
            case "Dash":
                HasDash = true;
                break;
            case "HasMagic":
                HasMagic = true;
                break;
            case "MagicLevel":
                MagicLevel += 1;
                break;
            default:
                Debug.LogWarning("Unknown ability: " + ability);
                break;
        }
    }

    public void UpdateMoney(int money)
    {
        Money += money;
    }

    public void CollectSphere(string sphereType)
    {
        switch (sphereType)
        {
            case "Sound":
                CollectedSpheresSound = true;
                break;
            case "Light":
                CollectedSpheresLight = true;
                break;
            default:
                Debug.LogWarning("Unknown sphere type: " + sphereType);
                break;
        }

    }

    public void LoadPlayerData(PlayerDataSave saveData)
    {
        HasSword = saveData.HasSword;
        SwordLevel = saveData.SwordLevel;
        HasWallClimb = saveData.HasWallClimb;
        HasDash = saveData.HasDash;
        HasMagic = saveData.HasMagic;
        MagicLevel = saveData.MagicLevel;
        Money = saveData.Money;
        CollectedSpheresSound = saveData.CollectedSpheresSound;
        CollectedSpheresLight = saveData.CollectedSpheresLight;
    }

    public void Load()
    {
        PlayerDataSave saveData = SaveSystem.LoadGame();
        if (saveData != null)
        {
            LoadPlayerData(saveData);
        }
    }

    public void ResetPlayerData()
    {
        HasSword = false;
        SwordLevel = 1;
        HasWallClimb = false;
        HasDash = false;
        HasMagic = false;
        MagicLevel = 1;
        Money = 0;
        CollectedSpheresSound = false;
        CollectedSpheresLight = false;
    }
}

