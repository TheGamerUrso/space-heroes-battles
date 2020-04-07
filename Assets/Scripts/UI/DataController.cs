using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataController : Singleton<DataController>
{
    private GameManager gm;
    public PlayerData playerData;
    private GameSettings gameSettings;

    private static MissionCollection missionCollection;
    private static LevelObjectiveCollection LevelObjectiveCollection;

    private bool firstRun;

    public void Setup(GameManager gm,int playerShips = 3)
    {
        this.gm = gm;

        missionCollection = JsonSystem.LoadMissions();
        LevelObjectiveCollection = JsonSystem.LoadLevelObjectiveData();
        playerData = new PlayerData(playerShips);


        int firstRunIndex = 0;

        if (PlayerPrefs.HasKey("FirstRun"))
        {
            firstRunIndex = PlayerPrefs.GetInt("FirstRun");
        }

        if (firstRunIndex == 1)
        {
            SaveSystem.LoadGame();

            GameSettings.Initialize(
                playerData.SFXVolume, 
                playerData.MusicVolume, 
                playerData.AutoAttack, 
                playerData.mute, 
                playerData.distance);


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
        else if (firstRunIndex == 0)
        {
            PlayerPrefs.SetInt("FirstRun", 1);
            SaveSystem.SaveGame();
        }

        GenerateLevelObjectiveData();



    }

    public Dictionary<string, LevelObjectiveData[]> GetListOfLevelChallanges()
    {
        return playerData.ListOfLevelChallenges;
    }
    public LevelObjectiveData[] GetLevelChallegeById(string levelId)
    {
        return GetLevelObjectivesByID(levelId);
    }

    public LevelObjectiveData[] GetLevelObjectivesByID(string levelId)
    {
        LevelObjectiveData[] objectives;
        if (playerData.ListOfLevelChallenges.TryGetValue(levelId, out objectives))
        {
            return objectives;
        }

        return null;
    }
    public void SetPlayerData(PlayerData playerData)
    {
        this.playerData = playerData;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }


    public Mission GetMission(int index)
    {
        return missionCollection.GetMission(index);
    }

    public MissionCollection GetMissionCollection()
    {
        return missionCollection;
    }

    public int GetNumberOfData()
    {
        return GetListOfLevelChallanges().Count;
    }

    public void GenerateLevelObjectiveData()
    {
        if (playerData.ListOfLevelChallenges.Count == 0)
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

                playerData.AddToListLevelChallenges(LevelObjectiveCollection.LevelObjective.Levels[i].ID, objectiveListData);
            }
        }
    }

}