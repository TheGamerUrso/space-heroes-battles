using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEditor.SearchService;
using UnityEngine;
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
    public int CurrentTotalEnemies;
    public int availableEnemies;
    public bool HasBoss;
    public int numberOfEnemiesEachWave;
    public GameObject BossPrefab;
    [SerializeField] protected List<EnemyElement> enemyElements;
    [SerializeField] protected float cooldown = 1f;

    protected bool BossWave = false;
    protected GameObject enemGO = null;
    protected GameObject currentBoss;
    protected float delay = 0.5f;
    protected WaitForSeconds shortDelay;
    protected WaitForSeconds CooldownTimer;
    public int EnemySpawnInTotal;

    protected WaitForSeconds shortWait = new WaitForSeconds(1);
    protected WaitForSeconds longWait = new WaitForSeconds(2);
    protected WaitForSeconds RewardWait = new WaitForSeconds(5);


    [SerializeField] protected GameController gameController;
    [SerializeField] protected GuiManager guiManager;

    public void Start()
    {
        shortDelay = new WaitForSeconds(delay);
        CooldownTimer = new WaitForSeconds(cooldown);

        TotalEnemies = numberOfEnemiesEachWave * waves;
        CurrentTotalEnemies = TotalEnemies;
        EnemySpawnInTotal = TotalEnemies;

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
                    timer = 0.1f; // Replaces shortWait
                    currentLoopState = GameplayLoopState.PreWaveDelay;
                }
                break;
            case GameplayLoopState.PreWaveDelay:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    NewWave();
                    gameController.IsSlowMo = true;
                    currentLoopState = GameplayLoopState.SpawningEnemies;
                }
                break;
            case GameplayLoopState.SpawningEnemies:
                spawnCooldownTimer -= Time.deltaTime;
                if (spawnCooldownTimer <= 0f)
                {
                    if (CurrentTotalEnemies > 0)
                    {
                        TotalEnemies--;

                        var random = UnityEngine.Random.Range(0, SpawnPoints.Count);
                        enemGO = SpawnPoints[random].SpawnEnemyElement(availableEnemies);
                        CurrentTotalEnemies++;

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
                var numberOfEnemies = GameObject.FindObjectsOfType<Enemy>();
                if (TotalEnemies <= 0 || numberOfEnemies.Length == 0)
                {
                    TotalEnemies = 0;
                    timer = 2.0f; // Post-wave delay
                    currentLoopState = GameplayLoopState.BossWaveSetup;
                    guiManager.BossWarning();
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
                            CurrentTotalEnemies++;

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

        CurrentTotalEnemies = TotalEnemies;
        string[] transmitions = { "Wave:\n" + waves };
        guiManager.PlayTrasmition(transmitions, false);

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
    private void StartHyperspaceSequence()
    {
        active = true;
        currentLoopState = GameplayLoopState.HyperspaceTransition;
    }
}
