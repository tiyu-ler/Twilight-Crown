using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // [SerializeField] private int currentSaveID = 0;
    private float playTime = 0f;
    private PlayerDataSave dataSave;
    public bool Isgame;
    public GameObject Case, Sword;
    private void Awake()
    {
        dataSave = FindObjectOfType<PlayerDataSave>();
        if (dataSave.HasSword)
        {
            if (Case) Destroy(Case);
            if (Sword) Destroy(Sword);
        }
        SaveSystem.Init();
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
        dataSave.saveID = currentSaveID;
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
            // collectedSpheresSound = dataSave.CollectedSpheresSound,
            catBossKilled = dataSave.catBossKilled,
            
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
            
            // dataSave.CollectedSpheresSound = saveObject.collectedSpheresSound;
            dataSave.catBossKilled = saveObject.catBossKilled;
            
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