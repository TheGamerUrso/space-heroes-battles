using System.Collections.Generic;
using UnityEngine;

public static class PersistantData
{
    private static PlayerData playerData;
    private static GameSettings gameSettings;
    private static MissionCollection missionCollection;
    private static LevelObjectiveCollection LevelObjectiveCollection;

    public static void ReplacePlayerData(PlayerData playerData)
    {
        PersistantData.playerData = playerData;
    }

    public static void Load()
    {
        SaveSystem.LoadGame();

    }

    public static void Save()
    {
        SaveSystem.SaveGame();
    }

    public static void LoadData()
    {
        missionCollection = JsonSystem.LoadMissions();
        LevelObjectiveCollection = JsonSystem.LoadLevelObjectiveData();
        playerData = new PlayerData(3);

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
            int missionsCompleted = 1;
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
            PlayerPrefs.SetInt("SurvivalMode", 0);
            playerData.AddCoin(9999999);
            SaveSystem.SaveGame();
        }
        GenerateLevelObjectiveData();
    }

    public static Dictionary<string, LevelObjectiveData[]> GetListOfLevelChallanges()
    {
        return playerData.ListOfLevelChallenges;
    }
    public static LevelObjectiveData[] GetLevelChallegeById(string levelId)
    {
        return GetLevelObjectivesByID(levelId);
    }

    public static LevelObjectiveData[] GetLevelObjectivesByID(string levelId)
    {
        LevelObjectiveData[] objectives;
        if (playerData.ListOfLevelChallenges.TryGetValue(levelId, out objectives))
        {
            return objectives;
        }

        return null;
    }

    public static PlayerData GetPlayerData()
    {
        return playerData;
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
        if (playerData.ListOfLevelChallenges.Count == 0)
        {
            for (int i = 0; i < LevelObjectiveCollection.LevelObjective.Levels.Length; i++)
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
