using System;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;

[Serializable]
public class PersistantData : MonoSingleton<PersistantData>
{
    public Player_SO[] Players;

    public PlayerData playerData;
    public GameSettings gameSettings;
    public MissionCollection missionCollection;
    public LevelObjectiveCollection LevelObjectiveCollection;
    public List<Level> Levels = new List<Level>();
    public Dictionary<string, LevelObjectiveData[]> LevelChallenges = new Dictionary<string, LevelObjectiveData[]>();
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
        Instance.playerData = new PlayerData(Instance.Players);

        GenerateLevelObjectiveData();

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
                  Instance.playerData.ControlScene);

            LoadLevelProgress();

        }
        else if (firstRunIndex == 0)
        {
            PlayerPrefs.SetInt("FirstRun", 1);
            PlayerPrefs.SetInt("SurvivalMode", 0);

            SaveLevelProgress();

            SaveSystem.SaveGame();
        }

        InitializeLevels();
    }

    public static PlayerData GetPlayerData()
    {
        return Instance.playerData;
    }

    public static Level GetMission(int index)
    {
        int levelMission = index - (int)LevelEnum.Level1;
        return Instance.Levels[levelMission];
    }

    public static MissionCollection GetMissionCollection()
    {
        return Instance.missionCollection;
    }

    public static void GenerateLevelObjectiveData()
    {
        if (Instance.LevelChallenges.Count == 0)
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

                AddToListLevelChallenges(Instance.LevelObjectiveCollection.LevelObjective.Levels[i].ID, objectiveListData);
            }
        }
    }

    public static void RefreshLevels()
    {
        bool surivalLocked = Instance.playerData.SurvivalUnlocked;

        Level survival = Instance.Levels[0];
        survival.interactable = surivalLocked;
        survival.Locked = !surivalLocked;

        for (int i = 1; i < Instance.Levels.Count; i++)
        {
            var Level = Instance.Levels[i];

            Level level = Instance.Levels[i];
            Debug.Log(Level.mission.ID + " : " + (Instance.playerData.LevelUnlocked + (int)LevelEnum.Level0));
            if (Level.mission.ID <= (Instance.playerData.LevelUnlocked + (int)LevelEnum.Level0))
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
        var missionCollection = GetMissionCollection();
        bool surivalLocked = Instance.playerData.SurvivalUnlocked;
        LevelObjectiveData[] missionChallanges = GetLevelObjectives(LevelEnum.Level0.ToString().ToString());
        var level = new Level("Survival", new Mission() { ID = 3, Title = "Survival Mode", Description = "Survive as Much as you can", Level = 0 }, missionChallanges, 4, surivalLocked, surivalLocked);
        Instance.Levels.Add(level);
        for (int i = 0; i < missionCollection.Missions.Length; i++)
        {
            Mission missionItem = missionCollection.Missions[i];
            missionChallanges = GetLevelObjectives(((LevelEnum)missionItem.ID).ToString());
            level = new Level(missionItem.Title, missionItem, missionChallanges, missionItem.SpriteID, false, true);

            Instance.Levels.Add(level);
        }

        RefreshLevels();
    }

    private static void AddToListLevelChallenges(string id, LevelObjectiveData[] challenges)
    {
        Instance.LevelChallenges.Add(id, challenges);
    }

    public static Dictionary<string, LevelObjectiveData[]> GetListOfObjectives()
    {
        return Instance.LevelChallenges;
    }

    public static LevelObjectiveData[] GetLevelObjectives(string levelID)
    {
        LevelObjectiveData[] missionChallanges;
        if (Instance.LevelChallenges.TryGetValue(levelID, out missionChallanges))
        {
            return missionChallanges;
        }
        return missionChallanges;
    }

    public static void LoadLevelProgress()
    {
        Instance.LevelChallenges = Instance.playerData.GetListOfObjectives();
    }

    public static void SaveLevelProgress()
    {
        Instance.playerData.SetListOfObjectives(Instance.LevelChallenges);
    }
}
