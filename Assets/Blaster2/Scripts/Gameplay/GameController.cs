using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;

[Serializable]
public class EnemyElement
{
    public string Name;
    public PoolGameObjectType gameObjectType;
}

[Serializable]
public struct PlayerShipElement
{
    public string name;
    public GameObject prefab;
}

public enum GameState
{
    IDLE,INITIALIZING,START,TRANSMISSION, GAME, GAMEOVER, WIN
}

public class GameController : MonoSingleton<GameController>
{
    public event Action<GameState> OnGameStateValueChanged;
    public Action<int> OnGameCoinsPickedValueChanged;
    public GameState CurrentGameState { get; set; } = GameState.START;

    [Header("Config")]
    public bool HasAsteroids { get; set; }
    public bool IsFirstRun { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsTransmiting { get; set; }
    public bool IsSlowMo { get; set; }
    public float difficulty { get; set; }

    [Header("Gameplay Configuration")]
    public bool pause;
    protected bool active = false;
    protected bool rewardToClaim = false;
    public int TotalCoinsInGame;
    public int Multiplier = 1;

    public int EnemyKilled = 0;
    public int EnemyEscaped = 0;
    public int Score = 0;
    public int CoinPicked = 0;

    protected IDataService dataService;
    protected IAudioService audioService;
    protected IAppService appService;

    private GameObject currentPlayer;
    [SerializeField] private PlayerShipElement[] PlayerShips;

    protected Ship playerShip;
    protected PlayerData playerData;

    [SerializeField] protected GameMode gameMode;
    [SerializeField] protected AsteroidSpawner asteroidSpawner;
    [SerializeField] protected GuiManager guiManager;
    private float timer = 1;
    //=================================================================================
    protected override void CleanUp()
    {
        base.CleanUp();
        DOTween.Clear(true);
        DOTween.ClearCachedTweens();
    }
    //=================================================================================
    protected override void Init()
    {
        base.Init();
        IsSlowMo = false;
        Application.targetFrameRate = 60;
    }
    //=================================================================================
    protected override void Setup()
    {
        base.Setup();
        dataService = GameContext.Get<IDataService>();
        audioService = GameContext.Get<IAudioService>();
        appService = GameContext.Get<IAppService>();

        playerData = dataService.GetPlayerData();
        playerData.SetSuperMeter(0);
        playerData.ResetWeaponPowerUPCollected();

        gameMode.SetLevel(
            dataService.GetPlayerData().GetCurrentPlayerShipData().level);

        SetGameState(GameState.INITIALIZING);
    }
    //=================================================================================
    private void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.IDLE:
                break;
            case GameState.INITIALIZING:
      
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
                        playerShip.DisableFire();
                    }
                    timer = 2;             
                    SetGameState(GameState.START);
                }
                break;
            case GameState.START:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    playerShip.EnableFire();
                    SetGameState(GameState.TRANSMISSION);
                    timer = 5;
                }
                break;
            case GameState.TRANSMISSION:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    playerShip.EnableFire();
                    SetGameState(GameState.GAME);
                }
                break;
            case GameState.GAME:
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

        GetPlayer()?.ExitLevel();

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
    public virtual void OnEnemyDiedHandled(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(baseEnemy.Id))
        {
            var playerData = dataService.GetPlayerData();
            int PlayerLevel = playerData.GetCurrentPlayerShipData().level;
            int EnemyLevel = baseEnemy.Level;
            int levelDiffrence = PlayerLevel / EnemyLevel;
            if (levelDiffrence == 0) levelDiffrence = 1;
            float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;

            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);

            SetPlayerXP(XPEarned);
            SetScore(baseEnemy.EnemyData.EnemyValue);
            IncreaseMultiplier();

            GuiManager.CreateFloatingText("<color=" + "yellow" + ">" + XPEarned + "</color>" + "<color=" + "orange" + "> XP </color>", baseEnemy.transform.localPosition);

            playerData.SetPlayerKillsCounter(1);

            if (baseEnemy.GetComponent<BossEnemy>() == null) return;
            playerData.SetBossKilledCount();
        }
    }
    //=================================================================================
    public void OnEnemyEscapedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            if (baseEnemy.GetComponent<BossEnemy>()) return;
            EnemyEscaped++;
            DecreaseMultipler();
        }
    }
    //=================================================================================
    public void OnEnemyHitHandled(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            var playerData = dataService.GetPlayerData();
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
        }
    }
}
