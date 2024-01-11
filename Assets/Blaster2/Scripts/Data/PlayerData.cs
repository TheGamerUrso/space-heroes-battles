using System;
using System.Collections.Generic;
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

    public List<Achievement> Achievements;

    public PlayerData() { }

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

    public void NewGame()
    {
        SetSuperMeter(0);
        SetPowerPackCollected(0);
    }

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
        QuestSystem.Instance.SetQuestProgressByType(QuestTypeEnum.KILL, EnemyKilled);
    }

    public void SetPlayerGotHitCounter(bool value)
    {
        GotHitInGame = value;
        PlayedGame = value;
    }
    public void SetCurrentSelectShip(int select)
    {
        CurrrentSelectedShip = select;
        Events.OnShipSelectValueChanged?.Invoke(CurrrentSelectedShip);
    }

    public void SetControlSceme(int option)
    {
        ControlScene = option;
        Events.OnControlScemeChange?.Invoke();
    }

    public void SetPowerPackCollected(int ammount)
    {
        PowerPackCollected += ammount;
        if (PowerPackCollected > 5)
        {
            PowerPackCollected = 5;
        }
        Events.OnPowerPackCollected?.Invoke(PowerPackCollected);
    }

    public void SetSuperMeter(float ammount)
    {
        PowerUpLevel = ammount;
        if (PowerUpLevel > 1)
        {
            PowerUpLevel = 1;
        }
        Events.PowerUpLevelValueChanged?.Invoke(PowerUpLevel);
    }

    public void SetScore(float score)
    {
        Score += score;
        if (Score > HighScore)
        {
            HighScore = score;
        }
        Events.OnScoreValueChanged?.Invoke((int)Score);
        QuestSystem.Instance.SetQuestProgressByType(QuestTypeEnum.SCORE, (int)Score);
    }
    public float GetHighScore()
    {
        return HighScore;
    }
    public float GetScore()
    {
        return Score;
    }

    public QuestData GetOnGoingObjectiveById(QuestTypeEnum objectiveType)
    {
        for (int i = 0; i < ListOfPlayerActiveQuest.Count; i++)
        {
            if ((QuestTypeEnum)ListOfPlayerActiveQuest[i].questType == objectiveType)
            {
                return ListOfPlayerActiveQuest[i];
            }
        }
        return null;
    }

    public void AbstractCoins(int ammount)
    {
        Coins -= ammount;
        CoinSpend += ammount;
        if (Coins < 0)
        {
            Coins = 0;
        }
        Events.OnCoinValueChanged?.Invoke(Coins);
        QuestSystem.Instance.SetQuestProgressByType(QuestTypeEnum.SPEND, CoinSpend);
    }

    public void AddCoin(int ammount)
    {
        CoinPicked += ammount;
        Coins += ammount;
        if (Coins > 9999999)
        {
            Coins = 9999999;
        }
        Events.OnCoinValueChanged?.Invoke(Coins);
        Events.OnCoinValueChanged?.Invoke(CoinPicked);
    }

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

        QuestSystem.Instance.SetQuestProgressByType(QuestTypeEnum.SURVIVE, WaveSurvived);
    }

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
        Events.OnSuperUseValueChanged?.Invoke(SuperUsed);
        QuestSystem.Instance.SetQuestProgressByType(QuestTypeEnum.USE, SuperUsed);
    }

    public void SetBossKilledCount()
    {
        if(BountyKilled==1)return;
        BountyKilled = 1;
        QuestSystem.Instance.SetQuestProgressByType(QuestTypeEnum.BOUNTY, BountyKilled);
    }

    public PlayerShipData GetCurrentPlayerShipData()
    {
        return playerShipData[CurrrentSelectedShip];
    }

    public void EarnXP(float ammount)
    {
        PlayerShipData currentPlayerShipSelected = GetCurrentPlayerShipData();

        var Level = currentPlayerShipSelected.level;
        var XP = currentPlayerShipSelected.xp;
        var xpToLevel = currentPlayerShipSelected.xpToLevel;

        if (Level < 20)
        {
            XP += ammount;

            if (XP >= xpToLevel)
            {
                Level++;
                XP = 0;

                xpToLevel = (Level / 10 + Level % 10) * 100 * Mathf.Pow(10, Level / 10);

                Events.OnLevelValueChanged?.Invoke(GetCurrentPlayerShipData().level);

                AchievementSystem.instance.Report(13, Level);
            }
        }
        else
        {
            XP = 0;
            xpToLevel = 0;
        }

        currentPlayerShipSelected.level = Level;
        currentPlayerShipSelected.xp = XP;
        currentPlayerShipSelected.xpToLevel = xpToLevel;

        Events.OnXpValueChanged?.Invoke(currentPlayerShipSelected.level, currentPlayerShipSelected.xp, xpToLevel);
    }

    public void SetUpgrade(int upgrade, int value)
    {
        PlayerShipData currentPlayerShipSelected = playerShipData[CurrrentSelectedShip];
        currentPlayerShipSelected.Upgrades[upgrade] = value;
        SaveSystem.SaveGame();
    }

    public float GetPowerUpLevelPresentage()
    {
        return PowerUpLevel / 1;
    }
    public void ResetWeaponPowerUPCollected()
    {
        PowerPackCollected = 0;
    }
}

