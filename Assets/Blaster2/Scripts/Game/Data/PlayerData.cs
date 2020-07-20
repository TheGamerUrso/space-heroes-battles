using System;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;


[Serializable]
public class PlayerData
{
    [Header("Surval Mode")]
    public long SurvivalScore;
    public long SurvivalHighScore;

    [Header("Story Mode")]
    public float[] Score;
    public float[] HighScore;


    [Header("Progression")]
    [Range(1,9)]
    public int LevelUnlocked;
    public bool SurvivalUnlocked;
    public int[] UnlockedHeroes;

    [Header("Statistics")]
    public int Coins;
    public int Kills;
    public int CoinSpend;
    public int SuperUsed;
    public int WaveSurvived;

    public bool GotHitInGame;
    public bool PlayedGame;


    [Header("Stats")]
    [Range(0,1)]
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
    public float Distance;
    [Range(0, 20)]
    public int MaxLevel;
    public Dictionary<string, LevelObjectiveData[]> ListOfLevelChallenges = new Dictionary<string, LevelObjectiveData[]>();
    public List<ObjectiveData> ListOfOnGoingObjectives = new List<ObjectiveData>();

    public PlayerShipData[] playerShipData = new PlayerShipData[3];

    public PlayerData(int number = 3)
    {
        LevelUnlocked = 1;
        Score = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        HighScore = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        Coins = 0;
        Kills = 0;
        PowerUpLevel = 0;
        CoinSpend = 0;
        WaveSurvived = 0;
        SuperUsed = 0;
        GotHitInGame = false;
        PlayedGame = false;
        UnlockedHeroes = new int[4];
        UnlockedHeroes[0] = 1;
        SFXVolume = .7f;
        MusicVolume = .7f;
        AutoAttack = true;
        mute = false;
        Distance = 5;
        playerShipData = new PlayerShipData[3];
        for (int i = 0; i < playerShipData.Length; i++)
        {
            playerShipData[i] = new PlayerShipData();
        }
    }

    public void NewGame()
    {
        PowerUpLevel = 0;
        PowerPackCollected = 0;
    }

    public void RemoveCoin(int ammount)
    {
        Coins -= ammount;
        Events.OnCoinValueChanged?.Invoke(Coins);
    }

    public void IncreaseSuperUse()
    {
        SuperUsed++;
        Events.OnSuperUseValueChanged?.Invoke(SuperUsed);
    }

    public void SetCurrentSelectShip(int select)
    {
        CurrrentSelectedShip = select;
        Events.OnShipSelectValueChanged?.Invoke(CurrrentSelectedShip);
    }

    public void SetDistance(int option)
    {
        Distance = option;
        Events.OnDistanceValueChanged?.Invoke(Distance);
    }

    public void SetXP(float ammount)
    {
        PlayerShipData playerShipData = GetCurrentPlayerShipData();
        playerShipData.xp = ammount;
        Events.OnXpValueChanged?.Invoke(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
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

    public void SetPowerUpAmmount(float ammount)
    {
        PowerUpLevel += ammount;
        Events.PowerUpLevelValueChanged?.Invoke(PowerUpLevel);
    }

    public void SetSurvivalUnlockedLock(bool value)
    {
        if (SurvivalUnlocked)
        {
            return;
        }
        SurvivalUnlocked = value;
    }


    public void SetSurvivalScore(int score)
    {
        SurvivalScore = score;
        if (SurvivalScore > SurvivalHighScore)
        {
            SurvivalHighScore = SurvivalScore;
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
        CoinSpend = ammount;
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
        SuperUsed = 0;
    }

    public void AddCoin(int Ammount)
    {
        Coins += Ammount;
        if (Coins > 9999999)
        {
            Coins = 9999999;
        }
    }

    public PlayerShipData GetCurrentPlayerShipData()
    {
        return playerShipData[CurrrentSelectedShip];
    }

    public void EarnXP(float ammount)
    {
        PlayerShipData currentPlayerShipSelected = playerShipData[CurrrentSelectedShip];

        var Level = currentPlayerShipSelected.level;
        var XP = currentPlayerShipSelected.xp;
        var xpToLevel = currentPlayerShipSelected.xpToLevel;

        if (Level < 20)
        {
            if (XP >= xpToLevel)
            {
                Level++;
                XP = 0;
                xpToLevel = (Level / 10 + Level % 10) * 100 * Mathf.Pow(10, Level / 10);

                Events.OnLevelValueChanged?.Invoke(GetCurrentPlayerShipData().level);
            }
        }
        else
        {
            XP = 0;
            xpToLevel = 0;
            GooglePlayServicesManager.UnlockAchivement(EasyMobile.EM_GameServicesConstants.Achievement_Max_Power);
        }

        currentPlayerShipSelected.level = Level;
        currentPlayerShipSelected.xp = XP;
        currentPlayerShipSelected.xpToLevel = xpToLevel;
    }

    public void SetUpgrade(int upgrade, int value)
    {
        PlayerShipData currentPlayerShipSelected = playerShipData[CurrrentSelectedShip];
        currentPlayerShipSelected.Upgrades[upgrade] = value;
        SaveSystem.SaveGame();
    }
    public void IncreasePowerUp(float value)
    {
        PowerUpLevel += value;
        if (PowerUpLevel > 1)
        {
            PowerUpLevel = 1;
        }
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

