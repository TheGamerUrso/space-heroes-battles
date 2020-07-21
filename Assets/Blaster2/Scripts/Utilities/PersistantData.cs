using System;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;

[Serializable]
public class PersistantData : MonoSingleton<PersistantData>
{
    public PlayerData playerData;
    public GameSettings gameSettings;
    public MissionCollection missionCollection;
    public LevelObjectiveCollection LevelObjectiveCollection;
    public List<Level> Levels = new List<Level>();

    public static List<Level> GetLevels()
    {
        return Instance.Levels;
    }

    public static void ReplacePlayerData(PlayerData playerData)
    {
        Instance.playerData = playerData;
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
        Instance.missionCollection = JsonSystem.LoadMissions();
        Instance.LevelObjectiveCollection = JsonSystem.LoadLevelObjectiveData();
        Instance.playerData = new PlayerData(3);

        int firstRunIndex = 0;

        if (PlayerPrefs.HasKey("FirstRun"))
        {
            firstRunIndex = PlayerPrefs.GetInt("FirstRun");

        }

        if (firstRunIndex == 1)
        {
            SaveSystem.LoadGame();

            new GameSettings(
                  Instance.playerData.SFXVolume,
                  Instance.playerData.MusicVolume,
                  Instance.playerData.AutoAttack,
                  Instance.playerData.mute,
                  Instance.playerData.Distance);

            Dictionary<string, LevelObjectiveData[]> Challanges = Instance.playerData.GetListOfObjectives();
            int missionsCompleted = 1;
            foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
            {
                if (item.Value[0].completed == true)
                {
                    missionsCompleted++;
                }
            }

            Instance.playerData.LevelUnlocked = missionsCompleted;
        }
        else if (firstRunIndex == 0)
        {
            PlayerPrefs.SetInt("FirstRun", 1);
            PlayerPrefs.SetInt("SurvivalMode", 0);
            SaveSystem.SaveGame();
        }

        GenerateLevelObjectiveData();

        InitializeLevels();
    }

    public static Dictionary<string, LevelObjectiveData[]> GetListOfLevelChallanges()
    {
        return Instance.playerData.ListOfLevelChallenges;
    }
    public static LevelObjectiveData[] GetLevelChallegeById(string levelId)
    {
        return GetLevelObjectivesByID(levelId);
    }

    public static LevelObjectiveData[] GetLevelObjectivesByID(string levelId)
    {
        LevelObjectiveData[] objectives;
        if (Instance.playerData.ListOfLevelChallenges.TryGetValue(levelId, out objectives))
        {
            return objectives;
        }

        return null;
    }

    public static PlayerData GetPlayerData()
    {
        return Instance.playerData;
    }
    public static Mission GetMission(int index)
    {
        return Instance.missionCollection.GetMission(index);
    }
    public static MissionCollection GetMissionCollection()
    {
        return Instance.missionCollection;
    }
    public static int GetNumberOfData()
    {
        return GetListOfLevelChallanges().Count;
    }
    public static void GenerateLevelObjectiveData()
    {
        if (Instance.playerData.ListOfLevelChallenges.Count == 0)
        {
            for (int i = 0; i < Instance.LevelObjectiveCollection.LevelObjective.Levels.Length; i++)
            {
                int size = Instance.LevelObjectiveCollection.LevelObjective.Levels[i].Objectives.Length;
                LevelObjectiveData[] objectiveListData = new LevelObjectiveData[size];
                for (int x = 0; x < Instance.LevelObjectiveCollection.LevelObjective.Levels[i].Objectives.Length; x++)
                {
                    objectiveListData[x] = new LevelObjectiveData(
                         Instance.LevelObjectiveCollection.LevelObjective.Levels[i].Objectives[x].ID, Instance.LevelObjectiveCollection.LevelObjective.Levels[i].Objectives[x].Description);
                }

                Instance.playerData.AddToListLevelChallenges(Instance.LevelObjectiveCollection.LevelObjective.Levels[i].ID, objectiveListData);
            }
        }
    }

    public static void RefreshLevels()
    {
        bool surivalLocked = Instance.playerData.SurvivalUnlocked;

        Level survival = Instance.Levels[0];
        survival.interactable = surivalLocked;
        survival.Locked = !surivalLocked;

        for (int i = 1; i < (Instance.Levels.Count - 1); i++)
        {
            Mission missionItem = Instance.missionCollection.Missions[i];
            Level level = Instance.Levels[i];
            if (missionItem.ID <= (Instance.playerData.LevelUnlocked + 4))
            {
                level.interactable = true;
                level.Locked = false;
            }
            else
            {
                level.interactable = false;
                level.Locked = true;
            }
        }
    }

    private static void InitializeLevels()
    {
        PlayerData playerData = Instance.playerData;
        var missionCollection = PersistantData.GetMissionCollection();

        bool surivalLocked = Instance.playerData.SurvivalUnlocked;
        var level = new Level("Survival", new Mission() { ID = 3, Title = "Survival Mode", Description = "Survive as Much as you can", Level = 0 }, 4, surivalLocked, surivalLocked);
        Instance.Levels.Add(level);
        for (int i = 0; i < missionCollection.Missions.Length; i++)
        {
            Mission missionItem = missionCollection.Missions[i];

            level = new Level(missionItem.Title, missionItem, missionItem.SpriteID, false, true);

            Instance.Levels.Add(level);
        }

        RefreshLevels();
    }
}
