using System;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[Serializable]
public class PlayerEconomyData
{
    public int Coins;
    public int CoinSpend;
    public int CoinPicked;
    public PlayerEconomyData()
    {
        Coins = 0;
        CoinSpend = 0;
        CoinPicked = 0;
    }
}
[Serializable]
public class PlayerStatsData
{
    public float Score;
    public float HighScore;

    public PlayerStatsData()
    {
        Score = 0;
        HighScore = 0;
    }
}

[Serializable]
public class PlayerSettingsData
{
    public float SFXVolume;
    public float MusicVolume;
    public bool AutoAttack;
    public bool mute;
    [Range(1, 2)]
    public int ControlScene;
}

[Serializable]
public class PlayerData
{
    public PlayerStatsData playerStatsData;
    public PlayerSettingsData playerSettingsData;
    public PlayerEconomyData playerEconomyData;

    public int CurrrentSelectedShip;
    public int[] UnlockedHeroes;

    public List<QuestData> ListOfPlayerActiveQuest = new List<QuestData>();

    public PlayerShipData[] playerShipData = new PlayerShipData[3];

    public PlayerData(Player_SO[] players)
    {
        playerStatsData = new PlayerStatsData();
        playerSettingsData = new PlayerSettingsData();
        playerEconomyData = new PlayerEconomyData();

        playerShipData = new PlayerShipData[players.Length];
        UnlockedHeroes = new int[3] { 1, 0, 0 };

        for (int i = 0; i < players.Length; i++)
        {
            playerShipData[i] = new PlayerShipData(players[i]);
        }
    }

    public PlayerShipData GetCurrentPlayerShipData()
    {
        return playerShipData[CurrrentSelectedShip];
    }

    public void SetCurrentSelectShip(int select)
    {
        CurrrentSelectedShip = select;
    }


    public void UpdateScore(int Score)
    {
        playerStatsData.Score += Score;
    }


    public void UpdateHighScore()
    {
        playerStatsData.HighScore = playerStatsData.Score;
    }

    public void UpdateKills(int amount)
    {

    }

    public void UpdateSuperUsed(int amount)
    {

    }

    public void UpdateWaveSurvived(int amount)
    {

    }
    public void UpdateBountyKilled(int BountyIndex)
    {

    }
    public void UpdateEnemyKilled(int amount)
    {

    }



    public void UpdateCurrency(int amount)
    {
        playerEconomyData.Coins += amount;
        playerEconomyData.CoinPicked += amount;
    }
    public void UpdateSpendCurrency(int amount)
    {
        playerEconomyData.CoinSpend += amount;
    }

}

