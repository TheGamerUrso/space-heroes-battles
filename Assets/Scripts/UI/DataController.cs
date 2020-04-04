using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    private GameManager gm;
    private PlayerData playerData;
    private GameSettings gameSettings;

    private static MissionCollection missionCollection;
    private static LevelObjectiveCollection LevelObjectiveCollection;

    private bool firstRun;

    public DataController(GameManager gm)
    {
        if (instance == null)
        {
            this.gm = gm;
            instance = this;
        }
    }

    public void Setup(int playerShips = 3)
    {
        missionCollection = JsonSystem.LoadMissions();
        LevelObjectiveCollection = JsonSystem.LoadLevelObjectiveData();
        instance.playerData = new PlayerData(playerShips);


        int firstRunIndex = 0;

        if (PlayerPrefs.HasKey("FirstRun"))
        {
            firstRunIndex = PlayerPrefs.GetInt("FirstRun");
        }

        if (firstRunIndex == 1)
        {
            SaveSystem.LoadGame();

            GameSettings.Initialize(instance.playerData.SFXVolume, instance.playerData.MusicVolume, instance.playerData.AutoAttack, instance.playerData.mute, instance.playerData.distance);


            Dictionary<string, LevelObjectiveData[]> Challanges = instance.playerData.GetListOfObjectives();
            int missionsCompleted = 0;
            foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
            {
                if (item.Value[0].completed == true)
                {
                    missionsCompleted++;
                }
            }

            instance.playerData.LevelUnlocked = missionsCompleted;
        }
        else if (firstRunIndex == 0)
        {
            PlayerPrefs.SetInt("FirstRun", 1);
            SaveSystem.SaveGame();
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
            for (int i = 0; i < LevelObjectiveCollection.LevelObjective.Levels.Length - 1; i++)
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