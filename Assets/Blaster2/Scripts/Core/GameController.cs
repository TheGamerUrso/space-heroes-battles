using DG.Tweening;
using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEngine;



[Serializable]
public struct PlayerShipElement
{
    public string name;
    public GameObject prefab;
}

public enum GameState
{
    IDLE,SPAWN_PLAYER,INITIALIZING,START,TRANSMISSION, GAME, GAMEOVER, WIN
}

public class GameController : MonoBehaviour
{
    public event Action<GameState> OnGameStateValueChanged;
    public Action<int> OnGameCoinsPickedValueChanged;
    public Action<int> OnGameScoreValueChanged;
    public GameState CurrentGameState { get; set; } = GameState.START;

    [Header("Config")]
    public bool IsFirstRun { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsTransmiting { get; set; }
    public bool IsSlowMo { get; set; }

    [Header("Gameplay Configuration")]
    public bool pause;
    public int TotalCoinsInGame;
    public int Multiplier = 1;

    public int EnemyKilled = 0;
    public int EnemyEscaped = 0;
    public int Score = 0;
    public int CoinPicked = 0;

    protected IDataService dataService;
    protected IAudioService audioService;
    protected IAppService appService;
    protected IEventService eventService;

    private GameObject currentPlayer;
    [SerializeField] private PlayerShipElement[] PlayerShips;

    protected PlayerShip playerShip;
    protected PlayerData playerData;
    [SerializeField] protected CameraManager cameraManager;
    [SerializeField] protected WaveManager waveManager;
    [SerializeField] protected AsteroidSpawner asteroidSpawner;
    [SerializeField] protected GuiManager guiManager;
    private float timer = 1;

    //=================================================================================
    protected void Awake()
    {
        IsSlowMo = false;
        Application.targetFrameRate = 60;
    }
    //=================================================================================
    protected void Start()
    {
        dataService = GameContext.Get<IDataService>();
        audioService = GameContext.Get<IAudioService>();
        appService = GameContext.Get<IAppService>();
        eventService = GameContext.Get<IEventService>();

        playerData = dataService.GetPlayerData();
        playerData.SetSuperMeter(0);
        playerData.ResetWeaponPowerUPCollected();

        waveManager.SetLevel(dataService.GetPlayerData().GetCurrentPlayerShipData().level);

        SetGameState(GameState.SPAWN_PLAYER);

        eventService.Subscribe<EnemyDiedEvent>(OnEnemyDiedHandled);
        eventService.Subscribe<EnemyEscapedEvent>(OnEnemyEscapedCallback);
        eventService.Subscribe<EnemyHitEvent>(OnEnemyHitHandled);

    }
    private void OnDestroy()
    {
        eventService.Subscribe<EnemyDiedEvent>(OnEnemyDiedHandled);
        eventService.Subscribe<EnemyEscapedEvent>(OnEnemyEscapedCallback);
        eventService.Subscribe<EnemyHitEvent>(OnEnemyHitHandled);


        DOTween.Clear(true);
        DOTween.ClearCachedTweens();
    }

    //=================================================================================
    private void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.SPAWN_PLAYER:

                appService.SetGameState(TheGamerUrso.Core.GameStateEnum.GAME);
                NewGame();
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    if (GetPlayer() == null)
                    {
                        int shipSelected = playerData.CurrrentSelectedShip;
                        var player = CreatePlayer(shipSelected);
                        playerShip = player.GetComponentInChildren<PlayerShip>();
                        playerShip.weaponController.DisableFire();
                        var followPlayer = GameObject.FindAnyObjectByType<PlayerFollow>();
                        cameraManager.SetTarget(followPlayer != null ? followPlayer.gameObject : null);
                    }
                    timer = 2;
                    SetGameState(GameState.INITIALIZING);
                }
            break;
            case GameState.INITIALIZING:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    guiManager.Setup((PlayerShip)playerShip, playerData);
                    SetGameState(GameState.START);
                }
                break;
            case GameState.START:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    SetGameState(GameState.TRANSMISSION);
                    timer = 5;
                }
                break;
            case GameState.TRANSMISSION:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    playerShip.weaponController.EnableFire();
                    SetGameState(GameState.GAME);
                }
                break;
            case GameState.GAME:

                if(playerShip.healthComponent.CurrentHealth <= 0)
                {
                    GameOver();
                }
                break;
            case GameState.GAMEOVER:
                break;
            case GameState.WIN:
                break;
        }
    }
    //=================================================================================
    public void SetGameState(GameState nextGameState)
    {
        switch (nextGameState)
        {
            case GameState.GAMEOVER:
                guiManager.GameOver();
                break;
            case GameState.WIN:
                guiManager.Win();
                break;
        }
        CurrentGameState = nextGameState;
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
        SetGameState(GameState.GAMEOVER);

        Time.timeScale = 1.0f;
        playerData.GetCurrentPlayerShipData().Upgrades[(int)UpgradeTypeEnum.Shield] = 0;
        playerData.SetScore(Score);
        playerData.AddCoin(CoinPicked);

        eventService?.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.SCORE, value = Score });
    }

    //======================================================================================================================================================
    public void NewGame()
    {
        IsGameOver = false;
        Multiplier = 1;
        EnemyKilled = 0;
        EnemyEscaped = 0;
        Score = 0;
        CoinPicked = 0;
        waveManager.SetLevel(1 );
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
        guiManager.SetScoreMultipler(ultiplierTextToShow);
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
        asteroidSpawner.EnableAsteroids();
    }
    //=====================================================================================================================================================
    public void DeactivateAsteroid()
    {
        asteroidSpawner.DeactivateAsteroid();
    }
    //=====================================================================================================================================================
    public void OnNewEntryUploadedHandled(bool success)
    {      
        StartCoroutine(DelayGameOver());
    }
    //======================================================================================================================================================
    IEnumerator DelayGameOver()
    {
        SaveSystem.SaveGame();
        audioService.PlayMusic("GameOver", false);
        yield return new WaitForSeconds(2.0f);
    }
    //======================================================================================================================================================
    IEnumerator DelayWinScreen()
    {
        Time.timeScale = 1.0f;

        dataService.GetPlayerData().GetCurrentPlayerShipData().Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        audioService.PlayMusic("Victory", false);

        //GetPlayer()?.ExitLevel();

        yield return new WaitForSeconds(2.0f);
        //TODO GAME OVER
    }
    //=================================================================================
    public GameObject CreatePlayer(int id)
    {
        if (id >= PlayerShips.Length)
        {
            id = 0;
        }

        currentPlayer = GameObject.Instantiate(PlayerShips[id].prefab.gameObject);

        currentPlayer.SetActive(true);

        return currentPlayer;
    }
    //=================================================================================
    public PlayerShip GetPlayer()
    {
        if (currentPlayer == null)
        {
            return null;
        }
        return currentPlayer.GetComponentInChildren<PlayerShip>();
    }
    //=================================================================================
    public virtual void OnEnemyDiedHandled(EnemyDiedEvent enemyDied)
    {
        var playerData = dataService.GetPlayerData();
        int PlayerLevel = playerData.GetCurrentPlayerShipData().level;
        int EnemyLevel = enemyDied.Level;
        int levelDiffrence = PlayerLevel / EnemyLevel;
        if (levelDiffrence == 0) levelDiffrence = 1;
        float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;

        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);

        playerData.GetCurrentPlayerShipData().EarnXP(XPEarned);
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);

        SetScore(enemyDied.Value);
        IncreaseMultiplier();

        playerData.SetPlayerKillsCounter(1);

        if (!enemyDied.WasBoss) return;
        playerData.SetBossKilledCount();
    }
    //=================================================================================
    public void OnEnemyEscapedCallback(EnemyEscapedEvent enemyEscaped)
    {
        playerData.EnemyEscaped++;
        DecreaseMultipler();
    }
    //=================================================================================
    public void OnEnemyHitHandled(EnemyHitEvent enemyEscaped)
    {
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
    }
}
