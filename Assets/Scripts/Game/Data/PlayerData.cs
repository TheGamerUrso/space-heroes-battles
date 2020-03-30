using System;
using System.Collections.Generic;
using EasyMobile;


public delegate void DistanceChanged(float ammount);

[Serializable]
public class PlayerData
{
    public DistanceChanged distanceChanged;

    public float[] Score;
    public float[] HighScore;
    public int Coins;
    public int TotalKills;
    public int LevelUnlocked;
    public int TotalMoneySpend;
    public int TotalSuperUsed;
    public int WaveSurvived;
    public int m_EnemyKilled;
    public bool GotHitInGame;
    public bool PlayedGame;
    public int currentSelectedShip;
    public int[] UnlockedHeroes;

    public Dictionary<string, LevelObjectiveData[]> ListOfLevelChallenges = new Dictionary<string, LevelObjectiveData[]>();
    public List<ObjectiveData> ListOfOnGoingObjectives = new List<ObjectiveData>();

    public int Level;
    public float xp;
    public float xpToLevel;

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

    public int[] Upgrades;

    public PlayerData()
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
        Upgrades = new int[Enum.GetValues(typeof(UpgradeType)).Length - 1];
        Level = 1;
        xp = 0;
        xpToLevel = 100;

        SFXVolume = .7f;
        MusicVolume = .7f;
        AutoAttack = true;
        mute = false;
        distance = 3;
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

    public void SetUpgrade(UpgradeElement upgradeElement)
    {
        if (upgradeElement.upgradeData.MaxLevel > 0)
        {
            Upgrades[(int)(upgradeElement.upgradeData.upgradeType)] = upgradeElement.currentUpgradeIndex;
        }
        else if (upgradeElement.upgradeData.MaxLevel == 0)
        {
            Upgrades[((int)(upgradeElement.upgradeData.upgradeType) - 1)] = upgradeElement.currentUpgradeIndex;
        }
        SaveSystem.SavePlayerData();
    }

    public void SetGameSettings(GameSettings gameSettings)
    {
        SFXVolume = gameSettings.SFXVolume;
        MusicVolume = gameSettings.MusicVolume;
        AutoAttack = gameSettings.AutoAttack;
        mute = gameSettings.mute;
        distance = gameSettings.distance;
    }

    public void Save()
    {
        LevelSystem levelSystem = PlayerManager.GetPlayerByID(currentSelectedShip).prefab.GetLevelSystem();
        Level = levelSystem.GetLevel();
        xp = levelSystem.GetXP();
        xpToLevel = levelSystem.GetXpToLevel();
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


    public void EarnXP(float ammount)
    {
        LevelSystem levelSystem = PlayerManager.GetPlayerByID(currentSelectedShip).prefab.GetLevelSystem();
        levelSystem.AddXP((int)ammount);
        Level = levelSystem.GetLevel();
        xp = levelSystem.GetXP();
        xpToLevel = levelSystem.GetXpToLevel();
    }
}