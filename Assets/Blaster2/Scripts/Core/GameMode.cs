using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
public enum GameplayLoopState
{
    WaitingForStart,
    PreWaveDelay,
    SpawningEnemies,
    WaitingForEnemiesToClear,
    BossWaveSetup,
    BossBattleActive,
    RewardScreenActive,
    HyperspaceTransition,
    Ended
}

public class GameMode : MonoBehaviour
{
    public event Action<GameplayLoopState> OnGameplayLoopStateValueChanged;
    [Header("Loop States")]
    [SerializeField] private GameplayLoopState currentLoopState;

    private float timer;
    private float spawnCooldownTimer;
    private bool rewardToClaim;
    private bool active;

    [Header("Boss Configuration")]
    [SerializeField] protected GameObject[] BossFights;
    [SerializeField] protected List<EnemySpawner> SpawnPoints = new List<EnemySpawner>();



    public bool BossBattleInitiated;
    public int waves;
    public int difficulty;

    public int TotalEnemies;
    public int TotalAliveEnemies;
    public int numberOfEnemiesEachWave;

    public int availableEnemies;
    public bool HasBoss;
    public GameObject BossPrefab;
    [SerializeField] protected List<EnemyElement> enemyElements;
    [SerializeField] protected float cooldown = 1f;

    protected bool BossWave = false;
    protected GameObject enemGO = null;
    protected GameObject currentBoss;
    protected float delay = 0.5f;
    protected WaitForSeconds shortDelay;
    protected WaitForSeconds CooldownTimer;
    protected WaitForSeconds shortWait = new WaitForSeconds(1);
    protected WaitForSeconds longWait = new WaitForSeconds(2);
    protected WaitForSeconds RewardWait = new WaitForSeconds(5);


    [SerializeField] protected GameController gameController;
    [SerializeField] protected GuiManager guiManager;
    protected IDataService dataService;
    protected IEventService eventService;
    private PlayerData playerData;

    public void Start()
    {
        dataService = GameContext.Get<IDataService>();
        playerData = dataService.GetPlayerData();
        eventService = GameContext.Get<IEventService>();

        shortDelay = new WaitForSeconds(delay);
        CooldownTimer = new WaitForSeconds(cooldown);

        TotalEnemies = numberOfEnemiesEachWave * waves;
        TotalEnemies = numberOfEnemiesEachWave;
        waves = 0;
    }

    private void Update()
    {
        if (gameController.pause) return;

        switch (currentLoopState)
        {
            case GameplayLoopState.WaitingForStart:
                if (gameController.CurrentGameState == GameState.GAME)
                {
                    timer = 1;
                
                   
                    currentLoopState = GameplayLoopState.PreWaveDelay;
                }
                break;
            case GameplayLoopState.PreWaveDelay:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    gameController.IsSlowMo = true;
                    NewWave();
                    string[] transmition = { "Survive", "Goodluck" };
                    if (waves <= 1)
                    {
                        transmition = new string[] { "Survive", "Goodluck" };
                    }
                    else
                    {
                        transmition = new string[] { "Wave:\n" + waves };
                        eventService?.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.SURVIVE, value = waves });
                    }
                    guiManager.RecieveTransmition(transmition, waves <= 1 ? true : false);
                    currentLoopState = GameplayLoopState.SpawningEnemies;
                }
                break;
            case GameplayLoopState.SpawningEnemies:
                if (guiManager.IncomingTransmition) return;
                spawnCooldownTimer -= Time.deltaTime;
                if (spawnCooldownTimer <= 0f)
                {
                    if (TotalEnemies > 0)
                    {
                        TotalEnemies--;

                        var random = UnityEngine.Random.Range(0, SpawnPoints.Count);
                        enemGO = SpawnPoints[random].SpawnEnemyElement(availableEnemies);
                        TotalAliveEnemies++;

                        enemGO.GetComponent<Enemy>().OnEnemyDied += OnEnemyDiedHandled;
                        enemGO.GetComponent<Enemy>().OnEnemyEscaped += OnEnemyEscapedCallback;
                        enemGO.GetComponent<Enemy>().OnEnemyHit += OnEnemyHitHandled;

                        spawnCooldownTimer = cooldown;
                    }
                    else
                    {
                        // Transition when wave spawn pool is empty
                        currentLoopState = GameplayLoopState.WaitingForEnemiesToClear;
                    }
                }
                break;
            case GameplayLoopState.WaitingForEnemiesToClear:
                if (TotalAliveEnemies <= 0)
                {
                    TotalAliveEnemies = 0;
                    TotalEnemies = 0;
                    timer = 2.0f; // Post-wave delay
                    if (waves >= 10)
                    {
                        guiManager.BossWarning();
                        if (guiManager.IncomingTransmition) return;
                        currentLoopState = GameplayLoopState.BossWaveSetup;
                    }
                    else
                    {
                        currentLoopState = GameplayLoopState.PreWaveDelay;
                    }
                }
                break;
            case GameplayLoopState.BossWaveSetup:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (gameController.GetPlayer().CurrentHealth > 0)
                    {
                        if (BossWave)
                        {
                            timer = 1.0f; // longWait replacement

                            currentBoss = BossFights[UnityEngine.Random.Range(0, BossFights.Length)];
                            SpawnBoss(currentBoss, difficulty);
                            TotalAliveEnemies = 1;

                            currentLoopState = GameplayLoopState.BossBattleActive;
                        }
                        else
                        {
                            StartHyperspaceSequence();
                        }
                    }
                    else
                    {
                        gameController.IsGameOver = true;
                        currentLoopState = GameplayLoopState.Ended;
                    }
                }
                break;
            case GameplayLoopState.BossBattleActive:
                if (!BossBattleInitiated)
                {
                    if (!gameController.GetPlayer().IsAlive)
                    {
                        gameController.GameOver();
                        currentLoopState = GameplayLoopState.Ended;
                        return;
                    }

                    timer = 1.0f; // RewardWait replacement
                    currentLoopState = GameplayLoopState.RewardScreenActive;
                    rewardToClaim = true;
                }
                break;
            case GameplayLoopState.RewardScreenActive:
                timer -= Time.deltaTime;
                if (timer <= 0f && !rewardToClaim)
                {
                    timer = 1.0f;
                    currentLoopState = GameplayLoopState.HyperspaceTransition;
                }
                break;
            case GameplayLoopState.HyperspaceTransition:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    currentLoopState = GameplayLoopState.PreWaveDelay; // Loop back for next wave
                }
                break;
        }
    }

    //======================================================================================================================================================
    public void NewWave()
    {
        BossWave = false;

        TotalEnemies = numberOfEnemiesEachWave;

        waves++;       

        if (waves > 0 && waves % 4 == 0)
        {
            availableEnemies++;
            difficulty += 1;
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
    //=================================================================================
    public BossEnemy SpawnBoss(GameObject BossPrefab, int difficulty = 1)
    {
        GameObject currentBoss = GameObject.Instantiate(BossPrefab);
        currentBoss.name = BossPrefab.name;

        BossEnemy enemy = currentBoss.GetComponentInChildren<BossEnemy>();
        enemy.Id = currentBoss.name;
        enemy.SetStats(difficulty);


        enemy.OnEnemyDied += OnEnemyDiedHandled;
        enemy.OnEnemyEscaped += OnEnemyEscapedCallback;
        enemy.OnEnemyHit += OnEnemyHitHandled;

        return enemy;
    }
    //=================================================================================
    public void SetLevel(int level)
    {
        difficulty = level;
        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            SpawnPoints[i].enemyElements = enemyElements;
            SpawnPoints[i].LevelDifficulty = difficulty;
        }
    }
    //=================================================================================
    private void StartHyperspaceSequence()
    {
        active = true;
        currentLoopState = GameplayLoopState.HyperspaceTransition;
    }
    //=================================================================================
    public virtual void OnEnemyDiedHandled(Enemy baseEnemy)
    {
        baseEnemy.OnEnemyDied -= OnEnemyDiedHandled;
        baseEnemy.OnEnemyEscaped -= OnEnemyEscapedCallback;
        baseEnemy.OnEnemyHit -= OnEnemyHitHandled;

        TotalAliveEnemies--;

        var playerData = dataService.GetPlayerData();
        int PlayerLevel = playerData.GetCurrentPlayerShipData().level;
        int EnemyLevel = baseEnemy.Level;
        int levelDiffrence = PlayerLevel / EnemyLevel;
        if (levelDiffrence == 0) levelDiffrence = 1;
        float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;

        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);

        playerData.GetCurrentPlayerShipData().EarnXP(XPEarned);
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);

        gameController.SetScore(baseEnemy.EnemyData.EnemyValue);
        gameController.IncreaseMultiplier();

        GuiManager.CreateFloatingText("<color=" + "yellow" + ">" + XPEarned + "</color>" + "<color=" + "orange" + "> XP </color>", baseEnemy.transform.localPosition);

        eventService?.Publish(new QuestProgressEvent() {questTypeEnum = QuestTypeEnum.KILL,value = gameController.EnemyKilled });
        playerData.SetPlayerKillsCounter(1);

        if (baseEnemy.GetComponent<BossEnemy>() == null) return;
        playerData.SetBossKilledCount();

        eventService?.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.BOUNTY, value = playerData.BountyKilled });
    }
    //=================================================================================
    public void OnEnemyEscapedCallback(Enemy baseEnemy)
    {
        baseEnemy.OnEnemyDied -= OnEnemyDiedHandled;
        baseEnemy.OnEnemyEscaped -= OnEnemyEscapedCallback;
        baseEnemy.OnEnemyHit -= OnEnemyHitHandled;

        TotalAliveEnemies--;

        if (baseEnemy.GetComponent<BossEnemy>()) return;
        playerData.EnemyEscaped++;
        gameController.DecreaseMultipler();
    }
    //=================================================================================
    public void OnEnemyHitHandled(Enemy baseEnemy,int value,int times)
    {
        baseEnemy.OnEnemyDied -= OnEnemyDiedHandled;
        baseEnemy.OnEnemyEscaped -= OnEnemyEscapedCallback;
        baseEnemy.OnEnemyHit -= OnEnemyHitHandled;

        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
    }
}
