using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static string playerDataPath = Application.persistentDataPath + "/playerData.dat";

    public static void Delete()
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            Directory.Delete(Application.persistentDataPath, true);
        }
        else
        {
            Debug.Log("Data not found");
        }
    }

    public static void SavePlayerData()
    {
        FileStream file = new FileStream(playerDataPath, FileMode.OpenOrCreate);

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            PlayerData playerData = DataController.GetPlayerData();
            formatter.Serialize(file, playerData);
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

    public static void LoadPlayerData()
    {
        PlayerData playerData;

        if (!File.Exists(playerDataPath))
        {
            playerData = new PlayerData();
            DataController.Instance.SetPlayerData(playerData);
        }

        FileStream file = new FileStream(playerDataPath, FileMode.Open);

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            playerData = (PlayerData)formatter.Deserialize(file);
            DataController.Instance.SetPlayerData(playerData);
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
