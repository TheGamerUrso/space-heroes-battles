using DG.Tweening;
using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    public enum GameState
    {
        START, TRANSMISSION,GAME, GAMEOVER, WIN
    }
    public Action<int> OnGameCoinsPickedValueChanged;
    public GameState CurrentGameState { get; set; } = GameState.START;

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
    }
    //=================================================================================
    public void IncreaseMultiplier()
    {
        Multiplier++;
        if (Multiplier >= 5)
        {
            Multiplier = 5;
        }
    }
    //=================================================================================
    public void DecreaseMultipler()
    {
        Multiplier--;
        if (Multiplier < 0)
        {
            Multiplier = 0;
        }
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
                    //TODO Pause Game
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
                  //TODO Pause Game
                }
            }
        }
    }
    //=================================================================================
    public void OnDestroy()
    {
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
        var appService = GameContext.Get<IAppService>();
       // appService.SetGameState(GameStateEnum.GAME);
        NewGame();
        yield return new WaitForSeconds(1.0f);

        var playerService = GameContext.Get<PlayerManager>();
        if (playerService.GetPlayer() == null)
        {
            int shipSelected = playerData.CurrrentSelectedShip;
            var player = playerService.CreatePlayer(shipSelected);
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
        CurrentGameState = gameState;
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
        playerData.SetScore(Score);
        playerData.AddCoin(CoinPicked);        

        SetGameState(GameState.GAMEOVER);
    }
    //======================================================================================================================================================
    IEnumerator DelayGameOver()
    {      
        SaveSystem.SaveGame();
        audioService.PlayMusic("GameOver", false);
        yield return new WaitForSeconds(2.0f);
       //TODO GAME OVER
    }
    //======================================================================================================================================================
    IEnumerator DelayWinScreen()
    {
        Time.timeScale = 1.0f;

        dataService.GetPlayerData().GetCurrentPlayerShipData().Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        audioService.PlayMusic("Victory", false);

        var playerService = GameContext.Get<PlayerManager>();
        playerService.GetPlayer()?.ExitLevel();

        yield return new WaitForSeconds(2.0f);
        //TODO GAME OVER
    }
    //======================================================================================================================================================
    public void SetScore(int Score)
    {
        var score = Multiplier * Score;
        Score += score;
        if (Score >= int.MaxValue)
        {
            Score = int.MaxValue;
        }
        var ultiplierTextToShow = Multiplier > 1 ? $"{score} + (x {Multiplier} )" : $"{score}";
        GuiManager.SetScoreMultipler(ultiplierTextToShow);
        //TODO SCORE VALUE CHANGED
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

    internal bool IsTrasnmiting()
    {
        throw new NotImplementedException();
    }

    internal bool HyperspaceEnded()
    {
        throw new NotImplementedException();
    }
}
