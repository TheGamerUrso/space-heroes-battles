using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static string playerDataPath = Application.persistentDataPath + "/gameSave.dat";

    public static void Delete()
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            PlayerPrefs.DeleteAll();
            Directory.Delete(Application.persistentDataPath, true);
        }
        else
        {
            Debug.Log("Data not found");
        }
    }

    public static void SaveGame()
    {
        FileStream file = new FileStream(playerDataPath, FileMode.OpenOrCreate);
        GameSave gameSave = new GameSave();
        PlayerData playerData = DataController.GetPlayerData();
        if (playerData != null)
        {
            playerData.Save();
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, gameSave);
            }
            catch (SerializationException e)
            {
                Debug.LogError("There was an issue serializing this data:  " + e.Message);
            }
            finally
            {
                file.Close();
            }
        }
    }

    public static void LoadGame()
    {
        PlayerData playerData = new PlayerData();

        if (!File.Exists(playerDataPath))
        {
            playerData = new PlayerData();
            DataController.SetPlayerData(playerData);
        }

        FileStream file = new FileStream(playerDataPath, FileMode.Open);

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            GameSave gameSave = (GameSave)formatter.Deserialize(file);
            playerData.Load();
        }
        catch (SerializationException e)
        {
            Debug.LogError("There was an issue serializing this data:  " + e.Message);
        }
        finally
        {
            file.Close();
        }
    }

}
