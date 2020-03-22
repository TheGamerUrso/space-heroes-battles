using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DataController
{
    private static DataController instance;
    private static DataController Instance
    {
        get
        {
            return instance;
        }
    }

    private PlayerData playerData;

    private GameSettings gameSettings;

    private static MissionCollection missionCollection;
    private static LevelObjectiveCollection LevelObjectiveCollection;

    private bool firstRun;

    public DataController()
    {
        instance = this;
        missionCollection = JsonSystem.LoadMissions();
        LevelObjectiveCollection = JsonSystem.LoadLevelObjectiveData();
        playerData = new PlayerData();

        firstRun = true;

        if (PlayerPrefs.HasKey("FirstRun"))
        {
            firstRun = false;
        }

        if (firstRun == false)
        {
            SaveSystem.LoadPlayerData();
            gameSettings = new GameSettings(playerData.SFXVolume, playerData.MusicVolume, playerData.AutoAttack, playerData.mute, playerData.distance);


            Dictionary<string, LevelObjectiveData[]> Challanges = playerData.GetListOfObjectives();
            int missionsCompleted = 0;
            foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
            {
                if (item.Value[0].completed == true)
                {
                    missionsCompleted++;
                }
            }

            playerData.LevelUnlocked = missionsCompleted;
        }
        else if (firstRun)
        {
            PlayerPrefs.SetInt("FirstRun", 1);
            SaveSystem.SavePlayerData();
        }

        GenerateLevelObjectiveData();

    }

    public static Dictionary<string, LevelObjectiveData[]> GetListOfLevelChallanges()
    {
        return instance.playerData.ListOfLevelChallenges;
    }
    public static LevelObjectiveData[] GetLevelChallegeById(string levelId)
    {
        return GetLevelObjectivesByID(levelId);
    }

    public static LevelObjectiveData[] GetLevelObjectivesByID(string levelId)
    {
        LevelObjectiveData[] objectives;
        if (instance.playerData.ListOfLevelChallenges.TryGetValue(levelId, out objectives))
        {
            return objectives;
        }

        return null;
    }
    public static void SetPlayerData(PlayerData playerData)
    {
        instance.playerData = playerData;
    }

    public static PlayerData GetPlayerData()
    {
        if (instance == null)
        {
            return new PlayerData();
        }
        return instance.playerData;
    }


    public static Mission GetMission(int index)
    {
        return missionCollection.GetMission(index);
    }

    public static MissionCollection GetMissionCollection()
    {
        return missionCollection;
    }

    public static int GetNumberOfData()
    {
        return GetListOfLevelChallanges().Count;
    }

    public static void GenerateLevelObjectiveData()
    {
        if (instance.playerData.ListOfLevelChallenges.Count == 0)
        {
            for (int i = 0; i < LevelObjectiveCollection.LevelObjective.Levels.Length-1; i++)
            {
                int size = LevelObjectiveCollection.LevelObjective.Levels[i].Objectives.Length;
                LevelObjectiveData[] objectiveListData = new LevelObjectiveData[size];
                for (int x = 0; x < LevelObjectiveCollection.LevelObjective.Levels[i].Objectives.Length; x++)
                {
                    objectiveListData[x] = new LevelObjectiveData(
                        LevelObjectiveCollection.LevelObjective.Levels[i].Objectives[x].ID, LevelObjectiveCollection.LevelObjective.Levels[i].Objectives[x].Description);
                }

                instance.playerData.AddToListLevelChallenges(LevelObjectiveCollection.LevelObjective.Levels[i].ID, objectiveListData);
            }
        }
    }

}