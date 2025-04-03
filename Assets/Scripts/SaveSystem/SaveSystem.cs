using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    private const string SAVE_EXTENSION = ".json";

    public static void Init()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }
    public static void Save(int saveID, PlayerDataSavePackage saveObject)
    {
        string savePath = GetSaveFilePath(saveID);
        string json = JsonUtility.ToJson(saveObject, true);
        Debug.Log(json);
        File.WriteAllText(savePath, json);
    }

    public static PlayerDataSavePackage Load(int saveID)
    {
        string savePath = GetSaveFilePath(saveID);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerDataSavePackage saveObject = JsonUtility.FromJson<PlayerDataSavePackage>(json);
            return saveObject;
        }
        return null;
    }

    public static void DeleteSave(int saveID)
    {
        string savePath = GetSaveFilePath(saveID);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }

    public static bool SaveExists(int saveID)
    {
        return File.Exists(GetSaveFilePath(saveID));
    }

    private static string GetSaveFilePath(int saveID)
    {
        return SAVE_FOLDER + "SaveSlot_" + saveID + SAVE_EXTENSION;
    }
}
