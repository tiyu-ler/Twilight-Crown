using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
// [SerializeField] private int currentSaveID = 1; // Default to first save slot
    private float playTime = 0f;
    private PlayerDataSave dataSave;
    public bool Isgame;
    private void Awake()
    {
        SaveSystem.Init();
        dataSave = FindObjectOfType<PlayerDataSave>(); // Reference to the player
    }

    private void Update()
    {
        if (Isgame)
        {
            playTime += Time.deltaTime;
        }
    }

    public void SaveGame(int currentSaveID)
    {
        PlayerDataSavePackage saveObject = new PlayerDataSavePackage
        {
            saveID = currentSaveID,
            obeliskID = dataSave.ObeliskID,
            
            hasSword = dataSave.HasSword,
            swordLevel = dataSave.SwordLevel,
            
            hasWallClimb = dataSave.HasWallClimb,
            hasDash = dataSave.HasDash,
            hasMagic = dataSave.HasMagic,
            magicLevel = dataSave.MagicLevel,
            money = dataSave.Money,
            collectedSpheresSound = dataSave.CollectedSpheresSound,
            collectedSpheresLight = dataSave.CollectedSpheresLight,
            
            totalPlayTime = playTime
        };

        SaveSystem.Save(currentSaveID, saveObject);
    }

    public void LoadGame(int currentSaveID)
    {
        PlayerDataSavePackage saveObject = SaveSystem.Load(currentSaveID);
        if (saveObject != null)
        {
            dataSave.ObeliskID = saveObject.obeliskID;
            
            dataSave.HasSword = saveObject.hasSword;
            dataSave.SwordLevel = saveObject.swordLevel;
            
            dataSave.HasWallClimb = saveObject.hasWallClimb;
            dataSave.HasDash = saveObject.hasDash;
            dataSave.HasMagic = saveObject.hasMagic;
            dataSave.MagicLevel = saveObject.magicLevel;
            
            dataSave.Money = saveObject.money;
            
            dataSave.CollectedSpheresSound = saveObject.collectedSpheresSound;
            dataSave.CollectedSpheresLight = saveObject.collectedSpheresLight;
            
            playTime = saveObject.totalPlayTime;
        }
        else
        {
            dataSave.Default();
            SaveGame(currentSaveID);
        }
    }

    public List<float> GetSaveProfilesData(int saveID)
    {
        PlayerDataSavePackage saveObject = SaveSystem.Load(saveID);
        
        List<float> Profile = new List<float>();
        
        if (saveObject != null)
        {
            float ObeliskID = saveObject.obeliskID;
            float Money = saveObject.money;
            float PlayTime = saveObject.totalPlayTime;
            
            Profile.Add(ObeliskID);
            Profile.Add(Money);
            Profile.Add(PlayTime);

            return Profile;
        }
        return null;
    }
}