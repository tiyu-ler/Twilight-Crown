[System.Serializable]
public class PlayerDataSave
{
    public int obeliskID;
    public bool HasSword;
    public int SwordLevel;
    public bool HasWallClimb;
    public bool HasDash;
    public bool HasMagic;
    public int MagicLevel;
    public int Money;
    public bool CollectedSpheresSound;
    public bool CollectedSpheresLight;

    public PlayerDataSave(int obeliskID, bool HasSword, int SwordLevel, bool HasWallClimb, bool HasDash, bool HasMagic, int MagicLevel, int Money, bool CollectedSpheresSound, bool CollectedSpheresLight)
    {
        this.obeliskID = obeliskID;
        this.HasSword = HasSword;
        this.SwordLevel = SwordLevel;
        this.HasWallClimb = HasWallClimb;
        this.HasDash = HasDash;
        this.HasMagic = HasMagic;
        this.MagicLevel = MagicLevel;
        this.Money = Money;
        this.CollectedSpheresSound = CollectedSpheresSound;
        this.CollectedSpheresLight = CollectedSpheresLight;
    }
}
