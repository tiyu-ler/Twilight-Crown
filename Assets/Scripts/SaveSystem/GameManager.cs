using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // [SerializeField] private int currentSaveID = 0;
    public bool Isgame;

    [Header("Sword")]
    public GameObject Case;
    public GameObject Sword;
    public GameObject SwordPickUpText;
    public GameObject SwordLight;

    [Header("Wall Climb")]
    public GameObject WallhBag;
    public GameObject WallPickUpText;
    public GameObject WallLight;
    
    [Header("Dash")]
    public GameObject DashCrystal;
    public GameObject DashPickUpText;
    public GameObject DashLight;

    private float playTime = 0f;
    private PlayerDataSave dataSave;

    private void Awake()
    {
        Isgame = true;
        
        dataSave = FindObjectOfType<PlayerDataSave>();
        
        UpdatePickUpObjects();

        SaveSystem.Init();
    }

    public void UpdatePickUpObjects()
    {
        if (dataSave.HasSword)
        {
            if (Case) Destroy(Case);
            if (Sword) Destroy(Sword);
            if (SwordPickUpText) Destroy(SwordPickUpText);
            if (SwordLight) Destroy(SwordLight);
        }
        if (dataSave.HasDash)
        {
            WallhBag.GetComponent<ClimbPickUp>().enabled = false;
            if (WallPickUpText) Destroy(WallPickUpText);
            if (Sword) Destroy(WallLight);
        }
        if (dataSave.HasWallClimb)
        {
            if (DashCrystal) Destroy(DashCrystal);
            if (DashPickUpText) Destroy(DashPickUpText);
            if (DashLight) Destroy(DashLight);
        }
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
            
            totalPlayTime = playTime + dataSave.totalPlayTime
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