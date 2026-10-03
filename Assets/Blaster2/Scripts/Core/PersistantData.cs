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
        playerData = new PlayerData(Players);

        new GameSettings(
                 playerData.playerSettingsData.SFXVolume,
                 playerData.playerSettingsData.MusicVolume,
                 playerData.playerSettingsData.AutoAttack,
                 playerData.playerSettingsData.mute,
                 playerData.playerSettingsData.ControlScene);
    }

    public void Save()
    {
        //SaveSystem.SaveGame();
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }
}
