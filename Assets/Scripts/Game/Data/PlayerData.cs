using System;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;

public delegate void XpValueChanged(int level, float xp, float xpToLevel);
public delegate void DistanceChanged(float ammount);
public delegate void SuperUseValueChanged(float ammount);
public delegate void CoinValueChanged(int ammount);
public delegate void ShipSelectValueChanged(int selection);
[Serializable]
public class PlayerData
{
    [NonSerialized] public XpValueChanged OnXpValueChanged;
    [NonSerialized] public CoinValueChanged OnCoinValueChanged;
    [NonSerialized] public XpValueChanged OnLevelValueChanged;
    [NonSerialized] public DistanceChanged distanceChanged;
    [NonSerialized] public ShipSelectValueChanged OnShipSelectValueChanged;
    [NonSerialized] public SuperUseValueChanged OnSuperUseValueChanged;


    public float[] score;
    public float[] Score;
    public float[] HighScore;
    
    
    public int coins;
    public int Coins
    {
        get
        {
            return coins;
        }
        set
        {
            coins = value;
            OnCoinValueChanged?.Invoke(coins);
        }
    }


    public int TotalKills;
    public int LevelUnlocked;
    public int TotalMoneySpend;
    public int TotalSuperUsed;
    public int WaveSurvived;
    public int m_EnemyKilled;
    public bool GotHitInGame;
    public bool PlayedGame;

    public int superUsed;
    public int SuperUsed
    {
        get { return superUsed; }
        set { 
            superUsed = value;
            OnSuperUseValueChanged?.Invoke(superUsed);       
        }
    }


    public int currentSelectedShip;

    public int CurrrentSelectedShip
    {
        get
        {
            return currentSelectedShip;
        }

        set
        {
            currentSelectedShip = value;
            OnShipSelectValueChanged?.Invoke(value);
        }
    }


    public int[] UnlockedHeroes;

    public Dictionary<string, LevelObjectiveData[]> ListOfLevelChallenges = new Dictionary<string, LevelObjectiveData[]>();
    public List<ObjectiveData> ListOfOnGoingObjectives = new List<ObjectiveData>();

    public PlayerShipData[] playerShipData = new PlayerShipData[3];

    public float SFXVolume;
    public float MusicVolume;
    public bool AutoAttack;
    public bool mute;
    public float distance;

    public float Distance
    {
        get
        {
            return distance;
        }

        set
        {
            distance = value;
            distanceChanged?.Invoke(distance);
        }
    }
    public int Level
    {
        get
        {
            return GetCurrentPlayerShipData().level;
        }
        set
        {
            GetCurrentPlayerShipData().level = value;
            OnXpValueChanged?.Invoke(GetCurrentPlayerShipData().level, GetCurrentPlayerShipData().xp, GetCurrentPlayerShipData().xpToLevel);
            GameEventSystem.Call(PlayerEventType.Player_LevelUp);
        }
    }

    public float XP
    {
        get
        {
            return GetCurrentPlayerShipData().xp;
        }
        set
        {
            GetCurrentPlayerShipData().xp = value;
            OnXpValueChanged?.Invoke(GetCurrentPlayerShipData().level, GetCurrentPlayerShipData().xp, GetCurrentPlayerShipData().xpToLevel);
        }
    }

    public PlayerData(int number = 3)
    {
        LevelUnlocked = 0;
        Score = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        HighScore = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        Coins = 0;
        TotalKills = 0;
        TotalMoneySpend = 0;
        WaveSurvived = 0;
        TotalSuperUsed = 0;
        m_EnemyKilled = 0;
        GotHitInGame = false;
        PlayedGame = false;
        UnlockedHeroes = new int[4];
        UnlockedHeroes[0] = 1;
        SFXVolume = .7f;
        MusicVolume = .7f;
        AutoAttack = true;
        mute = false;
        distance = 5;
        playerShipData = new PlayerShipData[3];
        for (int i = 0; i < playerShipData.Length; i++)
        {
            playerShipData[i] = new PlayerShipData();
        }
    }

    public void SetScore(int level, float score)
    {
        if (level < Score.Length && level >= 0)
        {
            if (score > Score[level])
            {
                HighScore[level] = score;
                if (GooglePlayServicesManager.Instance)
                    GooglePlayServicesManager.Instance.ReportLeaderboards((long)score, EM_GameServicesConstants.Leaderboard_Survival_Mode);

            }
            Score[level] = score;
        }
    }
    public float GetHighScore(int level)
    {
        if (level < HighScore.Length && level >= 0)
        {
            return HighScore[level];
        }
        return -1;
    }
    public float GetScore(int level)
    {
        if (level < Score.Length && level >= 0)
        {
            return Score[level];
        }
        return -1;
    }

    public ObjectiveData GetOnGoingObjectiveById(ObjectiveType objectiveType)
    {
        for (int i = 0; i < ListOfOnGoingObjectives.Count; i++)
        {
            if ((ObjectiveType)ListOfOnGoingObjectives[i].objectiveType == objectiveType)
            {
                return ListOfOnGoingObjectives[i];
            }
        }
        return null;
    }

    public void AddToListLevelChallenges(string id, LevelObjectiveData[] challenges)
    {
        ListOfLevelChallenges.Add(id, challenges);
    }

    public Dictionary<string, LevelObjectiveData[]> GetListOfObjectives()
    {
        return ListOfLevelChallenges;
    }

    public LevelObjectiveData[] GetLevelObjectivesByID(string levelId)
    {
        LevelObjectiveData[] objectives;
        if (ListOfLevelChallenges.TryGetValue(levelId, out objectives))
        {
            return objectives;
        }
        return null;
    }

    public LevelObjectiveData[] GetLevelObjectives(string levelID)
    {
        return GetLevelObjectivesByID(levelID);
    }

    public void SetGameSettings()
    {
        SFXVolume = GameSettings.SFXVolume;
        MusicVolume = GameSettings.MusicVolume;
        AutoAttack = GameSettings.AutoAttack;
        mute = GameSettings.mute;
        distance = GameSettings.distance;
    }

    public void SetMoneySpend(int ammount)
    {
        TotalMoneySpend = ammount;
    }

    public void SetWaveSurvived(int value)
    {
        WaveSurvived = value;
    }

    public void SetHitInGame(bool value)
    {
        GotHitInGame = value;
        PlayedGame = value;
    }

    public void SetTotalSuperUsed(int ammount)
    {
        TotalSuperUsed = 0;
    }

    public void AddCoin(int Ammount)
    {
        Coins += Ammount;
        if (Coins > 9999)
        {
            Coins = 9999;
        }
    }
    public PlayerShipData GetCurrentPlayerShipData(int selection)
    {
        return playerShipData[selection];
    }

    public PlayerShipData GetCurrentPlayerShipData()
    {
        return playerShipData[currentSelectedShip];
    }
    public void EarnXP(float ammount)
    {
        PlayerShipData playerShipData1 = playerShipData[currentSelectedShip];
        if (Level < playerShipData1.MaxLevel)
        {
            XP += ammount;
            if (XP >= playerShipData1.xpToLevel)
            {
                Level++;
                XP -= playerShipData1.xpToLevel;
                playerShipData1.xpToLevel = (Level / 10 + Level % 10) * 100 * Mathf.Pow(10, Level / 10);
            }
        }
        else
        {
            Level = playerShipData1.MaxLevel;
            XP = 0;
        }
    }

    public void SetUpgrade(int upgrade,int value)
    {
        PlayerShipData playerShipData1 = playerShipData[currentSelectedShip];
        playerShipData1.Upgrades[upgrade] = value;
        SaveSystem.SaveGame();
    }

}