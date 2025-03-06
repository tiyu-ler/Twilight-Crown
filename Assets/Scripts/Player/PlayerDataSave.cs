using UnityEngine;

public class PlayerDataSave : MonoBehaviour
{
    public static PlayerDataSave Instance;
    public int saveID;
    public int ObeliskID; // used
    public bool HasSword; 
    public int SwordLevel;
    public bool HasWallClimb;
    public bool HasDash;
    public bool HasMagic;
    public int MagicLevel;
    public int Money;
    public bool CollectedSpheresSound;
    public bool CollectedSpheresLight;
    public float totalPlayTime;

    void Update()
    {
        Debug.Log(ObeliskID);
    } 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Default()
    {
        ObeliskID = 0;
        HasSword = false;
        SwordLevel = 0;
        HasWallClimb = false;
        HasDash = false;
        HasMagic = false;
        MagicLevel = 0;
        Money = 0;
        CollectedSpheresSound = false;
        CollectedSpheresLight = false;
        totalPlayTime = 0;
    }
}
