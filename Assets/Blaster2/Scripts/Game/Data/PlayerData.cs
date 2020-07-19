using System;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;


[Serializable]
public class PlayerData
{
    #region Player Statistics
    public long SurvivalScore;
    public long SurvivalHighScore;

    public float[] score;
    public float[] Score;
    public float[] HighScore;

    public int coins;
    public int TotalKills;
    public int LevelUnlocked;
    public bool survivalUnlocked;

    public int TotalMoneySpend;
    public int TotalSuperUsed;
    public int WaveSurvived;
    public int m_EnemyKilled;
    public bool GotHitInGame;
    public bool PlayedGame;
    public int superUsed;
    public int[] UnlockedHeroes;

    public float powerUpLevel = 0;
    public int powerPackCollected = 0;


    #endregion

    #region Player Settings
    public int currentSelectedShip;
    public float SFXVolume;
    public float MusicVolume;
    public bool AutoAttack;
    public bool mute;
    public float distance;
    #endregion

    #region Properties
    public int Coins
    {
        get
        {
            return coins;
        }
        set
        {
            coins = value;
            Events.OnCoinValueChanged?.Invoke(coins);
        }
    }

    public int SuperUsed
    {
        get { return superUsed; }
        set
        {
            superUsed = value;
            Events.OnSuperUseValueChanged?.Invoke(superUsed);
        }
    }

    public int CurrrentSelectedShip
    {
        get
        {
            return currentSelectedShip;
        }

        set
        {
            currentSelectedShip = value;
            Events.OnShipSelectValueChanged?.Invoke(value);
        }
    }

    public float Distance
    {
        get
        {
            return distance;
        }

        set
        {
            distance = value;
            Events.OnDistanceValueChanged?.Invoke(distance);
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
            Events.OnLevelValueChanged?.Invoke(Level);
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
            Events.OnXpValueChanged?.Invoke(GetCurrentPlayerShipData().level, GetCurrentPlayerShipData().xp, GetCurrentPlayerShipData().xpToLevel);
        }
    }

    public int PowerPackCollected
    {
        get
        {
            return powerPackCollected;
        }
        set
        {
            powerPackCollected = value;
            if (powerPackCollected > 5)
            {
                powerPackCollected = 5;
            }
            Events.OnPowerPackCollected?.Invoke(powerPackCollected);
        }
    }

    public float PowerUpLevel
    {
        get
        {
            return powerUpLevel;
        }

        set
        {
            powerUpLevel = value;
            Events.PowerUpLevelValueChanged?.Invoke(powerUpLevel);
        }
    }

    public bool SurvivalUnlocked
    {
        get
        {
            return survivalUnlocked;
        }

        set
        {
            survivalUnlocked = value;
        }
    }


    #endregion

    public int MaxLevel { get; set; }
    public Dictionary<string, LevelObjectiveData[]> ListOfLevelChallenges = new Dictionary<string, LevelObjectiveData[]>();
    public List<ObjectiveData> ListOfOnGoingObjectives = new List<ObjectiveData>();

    public PlayerShipData[] playerShipData = new PlayerShipData[3];

    public PlayerData(int number = 3)
    {
        LevelUnlocked = 1;
        Score = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        HighScore = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        Coins = 0;
        TotalKills = 0;
        powerUpLevel = 0;
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
                if (level == 0)
                {
                    GooglePlayServicesManager.ReportLeaderboards((long)score, EM_GameServicesConstants.Leaderboard_Survival_Mode);
                    Game.IsHightScore = true;
                }

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
        if (Coins > 9999999)
        {
            Coins = 9999999;
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
        PlayerShipData currentPlayerShipSelected = playerShipData[currentSelectedShip];
        if (Level < 20)
        {
            XP += ammount;
            if (XP >= currentPlayerShipSelected.xpToLevel)
            {
                Level++;
                XP = 0;
                currentPlayerShipSelected.xpToLevel = (Level / 10 + Level % 10) * 100 * Mathf.Pow(10, Level / 10);
            }
        }
        else
        {
            XP = 0;
            currentPlayerShipSelected.xpToLevel = 0;
            GooglePlayServicesManager.UnlockAchivement(EasyMobile.EM_GameServicesConstants.Achievement_Max_Power);
        }
    }

    public void SetUpgrade(int upgrade, int value)
    {
        PlayerShipData playerShipData1 = playerShipData[currentSelectedShip];
        playerShipData1.Upgrades[upgrade] = value;
        SaveSystem.SaveGame();
    }
    public void IncreasePowerUp(float value)
    {
        PowerUpLevel += value;
    }
    public float GetPowerUpLevelPresentage()
    {
        return PowerUpLevel;
    }
    public void ResetWeaponPowerUPCollected()
    {
        powerPackCollected = 0;
    }

}