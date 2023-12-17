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
        Instance.playerData = new PlayerData(Instance.Players);

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

        }
        else if (firstRunIndex == 0)
        {
            PlayerPrefs.SetInt("FirstRun", 1);
            PlayerPrefs.SetInt("SurvivalMode", 0);
            SaveSystem.SaveGame();
        }
    }

    public static PlayerData GetPlayerData()
    {
        return Instance.playerData;
    }
}
