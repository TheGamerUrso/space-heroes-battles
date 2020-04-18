public static class GameSession
{
    public delegate void CoinValueChanged(int coin);
    public static CoinValueChanged OnCoinValueChanged;

    public delegate void ScoreValueChanged(int score);
    public static ScoreValueChanged OnScoreValueChanged;


    public delegate void MultiplierChanged(int multiplier);
    public static MultiplierChanged OnMultiplierChanged;

    public static bool IsGameOver;
    public static int EnemySpawnInTotal { get; set; }

    public static int score;
    public static int Score
    {
        get
        {
            return score;
        }

        set
        {
            score += value;
            OnScoreValueChanged?.Invoke(score);
        }
    }

    public static int WaveSurvived { get; set; }

    private static int currentEnemyKilled;

    public static int CurrentEnemyKilled
    {
        get
        {
            return currentEnemyKilled;
        }

        set
        {

            currentEnemyKilled = value;
            PlayerData playerData = GameManager.Instance.GetPlayerData();
            ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Kill);
            if (objectiveData != null)
                objectiveData.UpdateProgress(currentEnemyKilled);
        }
    }


    public static int coinEarnInGame;
    public static int CoinEarnInGame
    {
        get
        {
            return coinEarnInGame;
        }

        set
        {
            coinEarnInGame = value;
            OnCoinValueChanged?.Invoke(coinEarnInGame);
        }

    }

    private static int superUsed;
    public static int SuperUsed
    {

        get
        {
            return superUsed;
        }

        set
        {
            superUsed = value;
            PlayerData playerData = GameManager.Instance.GetPlayerData();
            ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Use);
            if (objectiveData != null)
                objectiveData.UpdateProgress(superUsed);
        }
    }

    private static bool getDamaged;

    public static bool GotDamaged
    {
        get
        {
            return getDamaged;
        }

        set
        {
            getDamaged = value;
            PlayerData playerData = GameManager.Instance.GetPlayerData();
            ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
            if (objectiveData != null)
                objectiveData.UpdateProgress(1);
        }
    }

    public static int multiplier = 1;
    public static int Multiplier
    {
        get
        {
            return multiplier;
        }

        set
        {
            if (value < 4)
            {
                multiplier = value;
            }
            OnMultiplierChanged?.Invoke(multiplier);
        }

    }

    public static int enemyKilled;
    public static int enemyEscaped;
    public static bool SurvivalMode;


    public static bool Transmiting;

    public static void Reset()
    {
        IsGameOver = false;
        EnemySpawnInTotal = 0;
        score = 0;
        WaveSurvived = 0;
        currentEnemyKilled = 0;
        CoinEarnInGame = 0;
        superUsed = 0;
        getDamaged = false;
        multiplier = 1;
        enemyKilled = 0;
        enemyEscaped = 0;
    }
}