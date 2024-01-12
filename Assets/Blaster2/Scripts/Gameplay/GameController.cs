using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        START, GAME, GAMEOVER, WIN
    }

    private GameState currentGameState = GameState.START;

    public static GameState CurrentGameState
    {
        get
        {
            var gameControllerFound = GameObject.FindObjectOfType<GameController>();
            if (gameControllerFound == null)
            {
                return GameState.GAME;
            }
            return gameControllerFound.currentGameState;
        }
    }

    public static GameController Instance
    {
        get
        {
            var gameControllerFound = GameObject.FindObjectOfType<GameController>();
            if (gameControllerFound == null)
            {
                return null;
            }
            return gameControllerFound;
        }
    }
    [Header("Components")]
    [SerializeField] private GameObject EnemyWaypoints;
    public GameObject AsteroidBackgroundSpawner;
    public GameObject Tutorial;
    public GameObject GUI;

    [Header("Config")]
    public bool HasAsteroids;
    private Ship playerShip;
    private PlayerData playerData;
    private BaseGameMode baseGameMode;
    public bool IsFirstRun;
    public bool IsGameOver;
    public bool IsHightScore = false;
    public bool IsTransmiting;
    public bool UseSlowMo;
    public bool SlowMo;
    public int TotalCoinsInGame;
    public int Multiplier = 1;
    public int EnemySpawnInTotal;
    public int EnemyKilled = 0;
    public int EnemyEscaped = 0;
    public int Score = 0;
    public int CoinPicked = 0;

    public void NewGame()
    {
        IsGameOver = false;
        Multiplier = 1;
        EnemyKilled = 0;
        EnemyEscaped = 0;
        Score = 0;
        CoinPicked = 0;
    }
    //=================================================================================
    public void ResetMultiplier()
    {
        Multiplier = 1;
        Events.OnMultiplierChanged?.Invoke();
    }
    //=================================================================================
    public void IncreaseMultiplier()
    {
        Multiplier++;
        if (Multiplier >= 5)
        {
            Multiplier = 5;
        }
        Events.OnMultiplierChanged?.Invoke();
    }
    //=================================================================================
    public void DecreaseMultipler()
    {
        Multiplier--;
        if (Multiplier < 0)
        {
            Multiplier = 0;
        }
        Events.OnMultiplierChanged?.Invoke();
    }
    //=================================================================================
    private void OnApplicationFocus(bool focus)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!focus)
            {
                if (IsGameOver == false)
                {
                    GameManager.Instance.PauseTheGame(true);
                }
            }
        }
    }
    //=================================================================================
    private void OnApplicationPause(bool Paused)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Paused)
            {
                if (IsGameOver == false)
                {
                    GameManager.Instance.PauseTheGame(true);
                }
            }
        }
    }
    //=================================================================================
    public void OnDestroy()
    {
        Events.PlayerLost -= GameOver;
        Events.GameEnded -= Win;

        DOTween.Clear(true);
        DOTween.ClearCachedTweens();
    }
    //=================================================================================
    protected void Awake()
    {
        Events.PlayerLost += GameOver;
        Events.GameEnded += Win;

        UseSlowMo = false;
        Application.targetFrameRate = 60;

        baseGameMode = GameObject.FindObjectOfType<BaseGameMode>();
        if (EnemyWaypoints != null) Instantiate(EnemyWaypoints, transform, false);
        if (AsteroidBackgroundSpawner != null && HasAsteroids) Instantiate(AsteroidBackgroundSpawner, transform, false);
        if (Tutorial != null) Instantiate(Tutorial, transform, false);
    }
    //=================================================================================
    void Start()
    {
        playerData = PersistantData.GetPlayerData();
        playerData.SetSuperMeter(0);
        playerData.ResetWeaponPowerUPCollected();
        SetGameState(GameState.START);
    }
    //=================================================================================
    IEnumerator StartGameDelay()
    {
        GameManager.Instance.ChangeGameState(GameStateEnum.GAME);
        NewGame();
        yield return new WaitForSeconds(1.0f);

        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = playerData.CurrrentSelectedShip;
            var player = PlayerManager.CreatePlayer(shipSelected);
            playerShip = player.GetComponentInChildren<PlayerShip>();
        }

        playerShip.DisableFire();
        yield return new WaitForSeconds(2.0f);
        playerShip.EnableFire();
        SetGameState(GameState.GAME);
    }
    //=================================================================================
    public void SetGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.START:
                StartCoroutine(StartGameDelay());
                break;
            case GameState.GAME:

                break;
            case GameState.GAMEOVER:
                if (IsGameOver == false)
                {
                    IsGameOver = true;
                    baseGameMode.GameOver();
                }
                break;
            case GameState.WIN:
                if (!IsGameOver)
                {
                    IsGameOver = true;
                    baseGameMode.Win();
                }
                break;
        }
        currentGameState = gameState;
    }
    //======================================================================================================================================================
    public void Win()
    {
        SetGameState(GameState.WIN);
    }
    //======================================================================================================================================================
    public void GameOver()
    {
        SetGameState(GameState.GAMEOVER);
    }
    //======================================================================================================================================================
    public static BaseGameMode GetGameMode()
    {
        return Instance.baseGameMode;
    }
    //======================================================================================================================================================
    public static void SetScore(int Score)
    {
        var score = Instance.Multiplier * Score;
        Instance.Score += score;
        var ultiplierTextToShow = Instance.Multiplier > 1 ? $"{score} + (x {Instance.Multiplier} )" : $"{score}";
        GuiManager.SetScoreMultipler(ultiplierTextToShow);
        Events.OnScoreValueChanged?.Invoke(Instance.Score);
    }
    //=====================================================================================================================================================
    public static void SetPlayerXP(float xp)
    {
        var playerData = PersistantData.GetPlayerData();
        playerData.EarnXP(xp);
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
    }
    //======================================================================================================================================================
    void OnGUI()
    {
        GUILayout.Label("Enemy Count " + Enemy.EnemiesCount);
    }
    //======================================================================================================================================================
}
