using System;

[Serializable]
public class PlayerDataSavePackage
{
    public int saveID;
    public int obeliskID;
    
    public bool hasSword;
    public int swordLevel;
    
    public bool hasWallClimb;
    public bool hasDash;
    public bool hasMagic;
    public int magicLevel;
    
    public int money;
    
    public bool catBossKilled;
    public bool secretZoneOpened;
    
    public float totalPlayTime; // in seconds
}
