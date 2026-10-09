using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerEconomyData
{
    public event Action<int> OnCurrencyValueChanged;
    public event Action<int> OnCurrencySpendValueChanged;
    public int Coins;
    public int CoinSpend;
    public int CoinPicked;
    public PlayerEconomyData()
    {
        Coins = 0;
        CoinSpend = 0;
        CoinPicked = 0;
    }

    public void UpdateCurrency(int amount)
    {
        Coins += amount;
        OnCurrencyValueChanged?.Invoke(amount);
        UpdateSpendCurrency(amount);
    }

    public void UpdateSpendCurrency(int amount)
    {
        OnCurrencySpendValueChanged?.Invoke(amount);
    }
}
[Serializable]
public class PlayerStatsData
{
    public  event Action<int> OnScoreValueChanged;
    public event Action<int> OnHighScoreValueChanged;
    public event Action<int> OnIsUnharmedValueChanged;
    public event Action<int> OnWaveSurvivedValueChanged;
    public event Action<int> OnBountyValueChanged;
    public event Action<int> OnSuperUsedValueChanged;
    public event Action<int> OnKillsValueChanged;
    public float Score;
    public float HighScore;
    public bool IsUnharmed;
    public int WaveSurvived;
    public int SpendCurrency;
    public int Bounty;
    public int SuperUsed;
    public int Kills;

    public PlayerStatsData()
    {
        Score = 0;
        HighScore = 0;
        IsUnharmed = false;
        WaveSurvived = 0;
        SpendCurrency = 0;
        Bounty = 0;
        SuperUsed = 0;
        Kills = 0;
    }
    public void UpdateScore(int Score)
    {
        this.Score += Score;
    }

    public void UpdateHighScore()
    {
        HighScore = Score;
    }

    public void UpdateKills(int amount)
    {
        Kills = amount;
    }

    public void UpdateSuperUsed(int amount)
    {
        SuperUsed = amount;
    }

    public void UpdateWaveSurvived(int amount)
    {
        WaveSurvived = amount;
    }
    public void UpdateIsUnharmed(bool IsUnharmed)
    {
        this.IsUnharmed = IsUnharmed;
    }

    public void UpdateBounty(int BountyIndex)
    {
        Bounty = BountyIndex;

    }
    public void UpdateEnemyKilled(int amount)
    {
        Kills = amount;
        OnKillsValueChanged?.Invoke(Kills);
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
    public event Action<int> OnCurrentShipSelectedValueChanged;
    public PlayerStatsData playerStatsData;
    public PlayerSettingsData playerSettingsData;
    public PlayerEconomyData playerEconomyData;

    public int CurrrentSelectedShip;
    public int[] UnlockedHeroes;

    public List<QuestData> ListOfPlayerActiveQuest = new List<QuestData>();

    public PlayerShipData[] playerShipData = new PlayerShipData[3];

    public bool IsUnharmed { get; internal set; }

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
        OnCurrentShipSelectedValueChanged?.Invoke(CurrrentSelectedShip);
    }
}

