using System;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;


[Serializable]
public class PlayerData
{
    public string Username;
    public float Score;
    public float HighScore;
    public int[] UnlockedHeroes;

    [Header("Statistics")]
    public int Coins;
    public int Kills;
    public int CoinSpend;
    public int SuperUsed = 0;
    public int WaveSurvived;
    public int CoinPicked;
    public bool PlayedGame;
    public bool GotHitInGame;
    public int BountyKilled;
    public int EnemyKilled;
    public int EnemyEscaped;

    [Header("Stats")]
    [Range(0, 1)]
    public float PowerUpLevel = 0;
    [Range(0, 4)]
    public int PowerPackCollected = 0;
    [Range(0, 2)]
    public int CurrrentSelectedShip;

    [Header("Settings")]
    public float SFXVolume;
    public float MusicVolume;
    public bool AutoAttack;
    public bool mute;
    [Range(1, 2)]
    public int ControlScene;
    [Range(0, 20)]
    public int MaxLevel;
    public List<QuestData> ListOfPlayerActiveQuest = new List<QuestData>();

    public PlayerShipData[] playerShipData = new PlayerShipData[3];

    public PlayerData(Player_SO[] players)
    {
        Score = 0;
        HighScore = 0;
        Coins = 0;
        Kills = 0;
        PowerUpLevel = 0;
        CoinSpend = 0;
        WaveSurvived = 0;
        SuperUsed = 0;
        GotHitInGame = false;
        PlayedGame = false;
        UnlockedHeroes = new int[3] { 1, 0, 0 };
        SFXVolume = .7f;
        MusicVolume = .7f;
        AutoAttack = true;
        mute = false;
        ControlScene = 1;
        playerShipData = new PlayerShipData[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerShipData[i] = new PlayerShipData(players[i]);
        }
    }
//======================================================================================================================================================
    public void SetPlayerKillsCounter(int KillsCounter)
    {
        if (KillsCounter == 0)
        {
            Kills = KillsCounter;
            EnemyKilled = KillsCounter;
        }
        else
        {
            Kills += KillsCounter;
            EnemyKilled += KillsCounter;
        }
    }
//======================================================================================================================================================
    public void SetPlayerGotHitCounter(bool value)
    {
        GotHitInGame = value;
        PlayedGame = value;
    }
    //======================================================================================================================================================
    public void SetCurrentSelectShip(int select)
    {
        CurrrentSelectedShip = select;
    }
//======================================================================================================================================================
    public void SetControlSceme(int option)
    {
        ControlScene = option;
    }
//======================================================================================================================================================
    public void SetPowerPackCollected(int ammount)
    {
        PowerPackCollected += ammount;
        if (PowerPackCollected > 5)
        {
            PowerPackCollected = 5;
        }
    }
//======================================================================================================================================================
    public void SetSuperMeter(float ammount)
    {
        PowerUpLevel = ammount;
        if (PowerUpLevel > 1)
        {
            PowerUpLevel = 1;
        }
    }
//======================================================================================================================================================
    public void SetScore(int score)
    {
        Score = score;
        Score = Mathf.Clamp(Score, 0, 999999999); 
    }
    //======================================================================================================================================================
    public void SetHighscore(int highscore)
    {
        HighScore = highscore;
        HighScore = Mathf.Clamp(HighScore, 0, 999999999);
    }
//======================================================================================================================================================
    public void AbstractCoins(int ammount)
    {
        Coins -= ammount;
        CoinSpend += ammount;
        if (Coins < 0)
        {
            Coins = 0;
        }
    }
//======================================================================================================================================================
    public void AddCoin(int ammount)
    {
        CoinPicked += ammount;
        Coins += ammount;
        if (Coins > 9999999)
        {
            Coins = 9999999;
        }
    }
//======================================================================================================================================================
    public void SetWaveSurvivedCount(int value)
    {
        if (value == 0)
        {
            WaveSurvived = value;
        }
        else
        {
            WaveSurvived += value;
        }
    }
//======================================================================================================================================================
    public void SetUsedSuperCount(int ammount)
    {
        if (ammount == 0)
        {
            SuperUsed = ammount;
        }
        else
        {
            SuperUsed += ammount;
        }
    }
//======================================================================================================================================================
    public void SetBossKilledCount()
    {
        if(BountyKilled==1)return;
        BountyKilled = 1;
    }
//======================================================================================================================================================
    public PlayerShipData GetCurrentPlayerShipData()
    {
        return playerShipData[CurrrentSelectedShip];
    }
    //======================================================================================================================================================
    public float GetPowerUpLevelPresentage()
    {
        return PowerUpLevel / 1;
    }
    //======================================================================================================================================================
    public void ResetWeaponPowerUPCollected()
    {
        PowerPackCollected = 0;
    }
}

