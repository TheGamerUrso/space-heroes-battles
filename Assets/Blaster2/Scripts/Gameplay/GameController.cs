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

public enum GameState
{
    START, TRANSMISSION, GAME, GAMEOVER, WIN
}

public class GameController : MonoSingleton<GameController>
{
 
    public Action<int> OnGameCoinsPickedValueChanged;
    public GameState CurrentGameState { get; set; } = GameState.START;

    [Header("Config")]

 
    public bool HasAsteroids { get; set; }
    public bool IsFirstRun { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsTransmiting { get; set; }
    public bool IsSlowMo { get; set; }
    public float difficulty { get; set; }


    [SerializeField] protected AsteroidSpawner asteroidSpawner;


    [Header("Boss Configuration")]
    [SerializeField] protected GameObject[] BossFights;
    [SerializeField] protected List<EnemySpawner> SpawnPoints = new List<EnemySpawner>();


    [Header("Gameplay Configuration")]
    public bool pause;
    public bool BossBattleInitiated;
    public int waves;
    public int LevelDifficulty;

    public int TotalEnemies;
    public int CurrentTotalEnemies;
    public int availableEnemies;

    public List<EnemyElement> enemyElements;
    public bool HasBoss;
    public int numberOfEnemiesEachWave;
    public GameObject BossPrefab;
    [SerializeField] protected float cooldown = 1f;

    protected bool BossWave = false;
    protected GameObject enemGO = null;
    protected GameObject currentBoss;


    protected WaitForSeconds shortDelay;
    protected WaitForSeconds CooldownTimer;
    protected WaitForSeconds shortWait = new WaitForSeconds(1);
    protected WaitForSeconds longWait = new WaitForSeconds(2);
    protected WaitForSeconds RewardWait = new WaitForSeconds(5);



    protected float delay = 0.5f;
    protected bool active = false;
    protected bool rewardToClaim = false;
    public int TotalCoinsInGame;
    public int Multiplier = 1;
    public int EnemySpawnInTotal;
    public int EnemyKilled = 0;
    public int EnemyEscaped = 0;
    public int Score = 0;
    public int CoinPicked = 0;

    protected IDataService dataService;
    protected IAudioService audioService;
    protected IGameService gameService;

    protected Ship playerShip;
    protected PlayerData playerData;

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

        playerData = dataService.GetPlayerData();
        playerData.SetSuperMeter(0);
        playerData.ResetWeaponPowerUPCollected();

        SetGameState(GameState.START);
    }
    //=================================================================================
    public void LateUpdate()
    {
        pause = false;
        if (Enemy.EnemiesCount >= 8)
        {
            pause = true;
        }
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
    public void SetGameMode()
    {
        shortDelay = new WaitForSeconds(delay);
        CooldownTimer = new WaitForSeconds(cooldown);

        TotalEnemies = numberOfEnemiesEachWave * waves;
        CurrentTotalEnemies = TotalEnemies;
        EnemySpawnInTotal = TotalEnemies;

        waves = 0;
        LevelDifficulty = dataService.GetPlayerData().GetCurrentPlayerShipData().level;
        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            SpawnPoints[i].enemyElements = enemyElements;
            SpawnPoints[i].LevelDifficulty = LevelDifficulty;
        }
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
        StopCoroutine(UpdateGameMode());

        Time.timeScale = 1.0f;
        playerData.GetCurrentPlayerShipData().Upgrades[(int)UpgradeTypeEnum.Shield] = 0;
        playerData.SetScore(Score);
        playerData.AddCoin(CoinPicked);        

        SetGameState(GameState.GAMEOVER);
    }
    //======================================================================================================================================================
    public void NewWave()
    {
        BossWave = false;

        CurrentTotalEnemies = TotalEnemies;

        waves++;

        string[] transmitions = { "Wave:\n" + waves };
        GuiManager.PlayTrasmition(transmitions, false);

        if (waves > 0 && waves % 4 == 0)
        {
            availableEnemies++;
            difficulty += .1f;
            if (availableEnemies > enemyElements.Count)
            {
                availableEnemies = enemyElements.Count;
            }

        }

        if (waves > 0 && waves % 2 == 0)
        {
            BossWave = true;
        }
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

    public bool IsTrasnmiting()
    {
        return false;   
    }
    //=================================================================================
    public bool HyperspaceEnded()
    {
        return false;
    }
    //=================================================================================
    public BossEnemy SpawnBoss(GameObject BossPrefab, int LevelDifficulty = 1)
    {
        audioService.PlayMusicById("Boss", true);
        GameObject currentBoss = GameObject.Instantiate(BossPrefab);
        currentBoss.name = BossPrefab.name;

        BossEnemy enemy = currentBoss.GetComponentInChildren<BossEnemy>();
        enemy.Id = currentBoss.name;
        enemy.SetStats(LevelDifficulty);

        return enemy;
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

            gameService.SetPlayerXP(XPEarned);
            gameService.SetScore(baseEnemy.EnemyData.EnemyValue);
            gameService.IncreaseMultiplier();

            GuiManager.CreateFloatingText("<color=" + "yellow" + ">" + XPEarned + "</color>" + "<color=" + "orange" + "> XP </color>", baseEnemy.transform.localPosition);

            playerData.SetPlayerKillsCounter(1);

            if (baseEnemy.GetComponent<BossEnemy>() == null) return;
            playerData.SetBossKilledCount();
            BossBattleInitiated = false;
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
    //=================================================================================
    public void Spawn()
    {
        CurrentTotalEnemies--;

        var random = UnityEngine.Random.Range(0, SpawnPoints.Count);
        enemGO = SpawnPoints[random].SpawnEnemyElement(availableEnemies);
        Enemy.EnemiesCount++;
    }
    //=================================================================================
    public void SpawnBoss()
    {
        if (!BossBattleInitiated)
        {
            BossBattleInitiated = true;

            currentBoss = BossFights[UnityEngine.Random.Range(0, BossFights.Length)];
            SpawnBoss(currentBoss, LevelDifficulty);
            Enemy.EnemiesCount++;
        }
    }
    //=================================================================================
    IEnumerator StartGameDelay()
    {
        var appService = GameContext.Get<IAppService>();
        appService.SetGameState(TheGamerUrso.Core.GameStateEnum.GAME);
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

        SetGameMode();
        yield return CooldownTimer;
        StartCoroutine(UpdateGameMode());
    }
    //=================================================================================
    public IEnumerator UpdateGameMode()
    {
        if (CurrentGameState == GameState.START)
        {
            yield return new WaitUntil(() => (CurrentGameState == GameState.GAME));
        }

        yield return shortWait;

        while (!IsGameOver)
        {
            NewWave();

            if (IsTrasnmiting() || CurrentGameState == GameState.GAME)
            {
                yield return new WaitUntil(() => !IsTrasnmiting());
            }

            yield return shortWait;

            IsSlowMo = true;

            if (pause)
            {
                yield return new WaitUntil(() => !pause);
            }

            while (CurrentTotalEnemies > 0)
            {
                while (pause)
                {
                    yield return shortWait;
                }

                if (IsGameOver) yield break;
                Spawn();
                yield return CooldownTimer;
                CooldownTimer = new WaitForSeconds(cooldown);
            }

            if (Enemy.EnemiesCount > 0)
            {
                while (Enemy.EnemiesCount > 0)
                {
                    yield return null;
                    var numberOfEnemies = GameObject.FindObjectsOfType<Enemy>();
                    if (numberOfEnemies.Length == 0)
                    {
                        yield return new WaitForSeconds(2.0f);
                        Enemy.EnemiesCount = 0;
                    }
                }
            }

            yield return shortWait;

            var playerService = GameContext.Get<PlayerManager>();
            if (playerService.GetPlayer().CurrentHealth > 0)
            {
                if (Enemy.EnemiesCount > 0)
                {
                    yield return new WaitUntil(() => Enemy.EnemiesCount <= 0);
                }

                if (BossWave)
                {
                    //GuiManager.Instance.BossWarning();

                    yield return longWait;

                    SpawnBoss();

                    if (BossBattleInitiated)
                    {
                        yield return new WaitUntil(() => !BossBattleInitiated);

                        if (!playerService.GetPlayer().IsAlive) yield break;

                        yield return RewardWait;

                        //GuiManager.Instance.ShowRewardScreen();
                        rewardToClaim = true;

                        while (rewardToClaim)
                        {
                            rewardToClaim = true;
                            yield return longWait;
                        }

                        yield return RewardWait;

                    }

                    active = true;
                }

                while (active)
                {
                    active = HyperspaceEnded();
                    yield return null;
                }

                yield return longWait;
            }
        }
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
}
