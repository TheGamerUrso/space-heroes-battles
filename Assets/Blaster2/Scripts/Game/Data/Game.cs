public struct Game
{
    public static int NumberOfEnemies = 0;


    public static int ShowAdCounter = 5;
    public static int MaxLevelUnlocked = 5;

    public static bool IsFirstRun;
    public static bool IsPaused;

    public static bool IsGameOver;
    public static bool IsHightScore = false;
    public static bool IsTransmiting;
    public static bool IsSurvivalMode;
    public static bool UseSlowMo;
    public static bool SlowMo;

    public static int Score;
    public static int SuperUsed = 0;
    public static int WaveSurvived;
    public static int CoinPicked;
    public static int TotalCoinsInGame;

    public static int Multiplier = 0;
    public static bool PlayerGotHit;

    public static int EnemyKilled;
    public static int EnemyEscaped;
    public static int EnemySpawnInTotal;
    public static void Reset()
    {
        IsGameOver = false;
        Score = 0;
        WaveSurvived = 0;
        CoinPicked = 0;
        SuperUsed = 0;
        PlayerGotHit = false;
        Multiplier = 0;
        EnemyKilled = 0;
        EnemyEscaped = 0;
        NumberOfEnemies = 0;
    }
    public static int GetScore()
    {
        return Score;
    }

    public static void SetScore(int value)
    {
        Score += value;
        Events.OnScoreValueChanged?.Invoke(Score);
    }

    public static void SetCoinPicked(int value)
    {
        CoinPicked = value;
        Events.OnCoinValueChanged?.Invoke(CoinPicked);
    }

    public static void GotHit()
    {
        if (PlayerGotHit == false)
        {
            PlayerGotHit = true;
        }
    }

    public static void SetSuperUsed(int value)
    {
        SuperUsed = value;
        GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.USE,SuperUsed);

    }
    public static void SetCurrentEnemyKills(int Kills)
    {
        EnemyKilled = Kills;
    }
    public static void ResetMultiplier()
    {
        Multiplier = 0;
        Events.OnMultiplierChanged?.Invoke();
    }

    public static void IncreaseMultiplier()
    {
        Multiplier++;
        if (Multiplier >= 5)
        {
            Multiplier = 5;
        }
        Events.OnMultiplierChanged?.Invoke();
    }


}