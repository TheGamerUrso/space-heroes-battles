using System;

using TheGamerUrso.Core;
using UnityEngine;


[Serializable]
[DefaultExecutionOrder(-100)]
public class PersistantData : ServiceComponent<IDataService>, IDataService
{
    public Player_SO[] Players;
    public PlayerData playerData;
    public GameSettings gameSettings;

    protected override void Awake()
    {
        base.Awake();
        Load();
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
            playerData = SaveSystem.LoadGame();

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
