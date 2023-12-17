using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PersistantData : MonoSingleton<PersistantData>
{
    public Player_SO[] Players;
    public AchievementList Achievements;
    public Sprite[] achievementIcons;

    public PlayerData playerData;
    public GameSettings gameSettings;
    public MissionCollection missionCollection;
    public LevelObjectiveCollection LevelObjectiveCollection;
    public Dictionary<string, LevelObjectiveData[]> LevelChallenges = new Dictionary<string, LevelObjectiveData[]>();
    
    public Sprite GetAchievementIcon(int id)
    {
        return achievementIcons[id];
    }
    
    public static void ReplacePlayerData(PlayerData playerData)
    {
        Instance.playerData = playerData;

        if (playerData.Achievements == null)
        {
            playerData.Achievements = Instance.Achievements.ListOfAchievelemtnts;
        }

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
    }

    public static PlayerData GetPlayerData()
    {
        return Instance.playerData;
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
