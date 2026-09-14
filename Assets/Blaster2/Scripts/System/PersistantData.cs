using System;

using TheGamerUrso.Core;
using UnityEngine;


[Serializable]
public class PersistantData : ServiceComponent<IDataService>, IDataService
{

public Player_SO[] Players;
    public Sprite[] achievementIcons;

    public PlayerData playerData;
    public GameSettings gameSettings;
    
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
        playerData = new PlayerData(Players);

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

    [ContextMenu("Add Coins")]
    public void AddCoin()
    {
        playerData.AddCoin(999);
    }

    [ContextMenu("Increase Super Power")]
    public void IncreaseSuperPower()
    {
      playerData.SetSuperMeter(playerData.PowerUpLevel + 1);
    }
}
