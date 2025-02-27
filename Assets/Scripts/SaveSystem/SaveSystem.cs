using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static string saveFilePath = Application.persistentDataPath + "/game.save";

    public static void SaveGame(
        int obeliskID,
        bool HasSword,
        int SwordLevel,
        bool HasWallClimb,
        bool HasDash,
        bool HasMagic,
        int MagicLevel,
        int Money,
        bool CollectedSpheresSound,
        bool CollectedSpheresLight)
    {
        PlayerDataSave data = new PlayerDataSave(
            obeliskID,
            HasSword,
            SwordLevel,
            HasWallClimb,
            HasDash,
            HasMagic,
            MagicLevel,
            Money,
            CollectedSpheresSound,
            CollectedSpheresLight
        );
        FileStream file = new FileStream(saveFilePath, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(file, data);

        file.Close();
    }

    public static PlayerDataSave LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            FileStream file = new FileStream(saveFilePath, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();

            PlayerDataSave data = (PlayerDataSave)formatter.Deserialize(file);

            file.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found!");
            return null;
        }
    }
}
