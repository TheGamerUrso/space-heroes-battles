using System;

using TheGamerUrso.Core;
using UnityEngine;


[Serializable]
[DefaultExecutionOrder(-100)]
public class PersistantData : ServiceComponent<IDataService>, IDataService
{

    public Player_SO[] Players;
    public Sprite[] achievementIcons;

    public PlayerData playerData;

    public GameSettings gameSettings;

    private void Start()
    {
        Load();
    }
    public Sprite GetAchievementIcon(int id)
    {
        return achievementIcons[id];
    }
    
    public void ReplacePlayerData(PlayerData playerData)
    {
        this.playerData = playerData;
    }


    public void Load()
    {
        int firstRunIndex = 0;

        if (PlayerPrefs.HasKey("FirstRun"))
        {
            firstRunIndex = PlayerPrefs.GetInt("FirstRun");

        }

        if (firstRunIndex == 1)
        {
            SaveSystem.LoadGame();

            new GameSettings(
                  playerData.SFXVolume,
                  playerData.MusicVolume,
                  playerData.AutoAttack,
                  playerData.mute,
                  playerData.ControlScene);

        }
        else if (firstRunIndex == 0)
        {
            playerData = new PlayerData(Players);
            PlayerPrefs.SetInt("FirstRun", 1);
            SaveSystem.SaveGame();
        }
    }

    public void Save()
    {
        SaveSystem.SaveGame();
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }
}
