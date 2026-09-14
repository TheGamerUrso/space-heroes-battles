using DG.Tweening;
using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEngine;

public class GameController : ServiceComponent<IGameService>, IGameService
{
    public enum GameState
    {
        START, GAME, GAMEOVER, WIN
    }
    public Action<int> OnGameCoinsPickedValueChanged;
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
    public float difficulty;
    private IDataService dataService;
    private IAudioService audioService;


    public void NewGame()
    {
        IsGameOver = false;
        Multiplier = 1;
        EnemyKilled = 0;
        EnemyEscaped = 0;
        Score = 0;
        CoinPicked = 0;
        difficulty = 1;
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
                    //GameManager.Instance.PauseTheGame(true);
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
                   // GameManager.Instance.PauseTheGame(true);
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
        dataService = GameContext.Get<IDataService>();
        audioService = GameContext.Get<IAudioService>();




        playerData = dataService.GetPlayerData();
        playerData.SetSuperMeter(0);
        playerData.ResetWeaponPowerUPCollected();


        Events.PlayerLost += GameOver;
        Events.GameEnded += Win;

        UseSlowMo = false;
        Application.targetFrameRate = 60;

        baseGameMode = GameObject.FindObjectOfType<BaseGameMode>();
        if (EnemyWaypoints != null) Instantiate(EnemyWaypoints, transform, false);
        if (AsteroidBackgroundSpawner != null) Instantiate(AsteroidBackgroundSpawner, transform, false);
        AsteroidBackgroundSpawner.SetActive(true);
        if (Tutorial != null) Instantiate(Tutorial, transform, false);
    }
    //=================================================================================
    void Start()
    {     
        SetGameState(GameState.START);
    }
    //=================================================================================
    IEnumerator StartGameDelay()
    {
        //GameManager.Instance.ChangeGameState(GameStateEnum.GAME);
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
                if (!IsGameOver)
                {
                    IsGameOver = true;
                }
                break;
            case GameState.WIN:
                if (!IsGameOver)
                {
                    IsGameOver = true;
                }
                break;
        }
        currentGameState = gameState;
    }
    //======================================================================================================================================================
    public void Win()
    {
        SetGameState(GameState.WIN);
        StartCoroutine(DelayWinScreen());
    }
    //======================================================================================================================================================
    public void GameOver()
    {
        Time.timeScale = 1.0f;
        playerData.GetCurrentPlayerShipData().Upgrades[(int)UpgradeTypeEnum.Shield] = 0;
        playerData.SetScore(GameController.Instance.Score);
        playerData.AddCoin(CoinPicked);        

        SetGameState(GameState.GAMEOVER);
    }
    //======================================================================================================================================================
    IEnumerator DelayGameOver()
    {      
        SaveSystem.SaveGame();
        audioService.PlayMusic("GameOver", false);
        yield return new WaitForSeconds(2.0f);
        Events.OnGameOver?.Invoke(false);
    }
    //======================================================================================================================================================
    IEnumerator DelayWinScreen()
    {
        Time.timeScale = 1.0f;

        dataService.GetPlayerData().GetCurrentPlayerShipData().Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        audioService.PlayMusic("Victory", false);

        PlayerManager.GetPlayer()?.ExitLevel();

        yield return new WaitForSeconds(2.0f);
        Events.OnGameOver?.Invoke(true);
    }
    //======================================================================================================================================================
    public BaseGameMode GetGameMode()
    {
        return Instance.baseGameMode;
    }
    //======================================================================================================================================================
    public void SetScore(int Score)
    {
        var score = Instance.Multiplier * Score;
        Instance.Score += score;
        if (Instance.Score >= int.MaxValue)
        {
            Score = int.MaxValue;
        }
        var ultiplierTextToShow = Instance.Multiplier > 1 ? $"{score} + (x {Instance.Multiplier} )" : $"{score}";
        GuiManager.SetScoreMultipler(ultiplierTextToShow);
        Events.OnScoreValueChanged?.Invoke(Instance.Score);
    }
    //=====================================================================================================================================================
    public void SetPlayerXP(float xp)
    {
        var playerData = dataService.GetPlayerData();
        playerData.EarnXP(xp);
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
    }
//=====================================================================================================================================================
    public void SetCoinPicked(int coinPicked)
    {
        CoinPicked += coinPicked;
        OnGameCoinsPickedValueChanged?.Invoke(CoinPicked);
    }
//=====================================================================================================================================================
    public void EnableAsteroids()
    {
        AsteroidBackgroundSpawner.GetComponent<AsteroidSpawner>().EnableAsteroids();
    }
//=====================================================================================================================================================
    public void DeactivateAsteroid()
    {
        AsteroidBackgroundSpawner.GetComponent<AsteroidSpawner>().DeactivateAsteroid();
    }
//=====================================================================================================================================================
    public void OnNewEntryUploadedHandled(bool success)
    {      
        StartCoroutine(DelayGameOver());
    }
}
