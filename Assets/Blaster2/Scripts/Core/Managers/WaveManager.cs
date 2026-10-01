using System;
using System.Collections.Generic;
using TheGamerUrso.Core;
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
    RewardClaimed,
    HyperspaceTransition,
    Ended
}

[Serializable]
public class WaveData
{
    public int Difficulty { get; set; }
    public bool BossBattleInitiated;
    public bool HasBoss;
    public int Wave;
    public float Cooldown = 1f;
    public float Delay = 0.5f;
    public float timer;
    public float spawnCooldownTimer;

    public int numberOfEnemiesEachWave = 5; // Default baseline
    public int enemiesSpawnedThisWave;     // Tracks how many have been spawned
    public int TotalAliveEnemies;           // Tracks active enemies via events

    public int availableEnemies;
    public bool HasAsteroids { get; set; }
    public GameObject UpcomingBossPrefab;
    public GameObject[] BossFights;
    public List<PoolGameObjectType> enemyElements;
}

public class WaveManager : MonoBehaviour
{
    public event Action<GameplayLoopState> OnGameplayLoopStateValueChanged;
    [SerializeField] private GameplayLoopState currentLoopState;
    [SerializeField] protected EnemySpawner enemySpawner;

    protected GameObject enemGO = null;
    protected GameObject currentBoss;
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
    public WaveData waveData;

    public Queue<PoolGameObjectType> waveSpawnQueue = new Queue<PoolGameObjectType>();
    public List<PoolGameObjectType> waveEnemyQueList = new List<PoolGameObjectType>();
    public void Start()
    {
        dataService = GameContext.Get<IDataService>();
        playerData = dataService.GetPlayerData();
        eventService = GameContext.Get<IEventService>();

        shortDelay = new WaitForSeconds(waveData.Delay);
        CooldownTimer = new WaitForSeconds(waveData.Cooldown);

        waveData.Wave = 0;

        eventService.Subscribe<EnemyDiedEvent>(OnEnemyDiedHandled);
        eventService.Subscribe<EnemyEscapedEvent>(OnEnemyEscapedCallback);
    }

    private void OnDestroy()
    {
        eventService.Unsubscribe<EnemyDiedEvent>(OnEnemyDiedHandled);
        eventService.Unsubscribe<EnemyEscapedEvent>(OnEnemyEscapedCallback);
    }

    private void Update()
    {
        if (gameController.pause) return;

        switch (currentLoopState)
        {
            case GameplayLoopState.WaitingForStart:
                if (gameController.CurrentGameState == GameState.GAME)
                {
                    waveData.timer = waveData.Cooldown;
                    currentLoopState = GameplayLoopState.PreWaveDelay;
                }
                break;
            case GameplayLoopState.PreWaveDelay:
                gameController.IsSlowMo = true;
                NewWave();
                string[] transmition = { "Survive", "Goodluck" };
                if (waveData.Wave <= 1)
                {
                    transmition = new string[] { "Survive", "Goodluck" };
                }
                else
                {
                    transmition = new string[] { "Wave:\n" + waveData.Wave };
                    eventService?.Publish(new NewWaveStartedEvent() { Wave = waveData.Wave });
                }
                guiManager.RecieveTransmition(transmition, waveData.Wave <= 1 ? true : false);
                currentLoopState = GameplayLoopState.SpawningEnemies;
                break;
            case GameplayLoopState.SpawningEnemies:
                if (guiManager.IncomingTransmition) return;
                waveData.timer -= Time.deltaTime;
                if (waveData.timer <= 0f)
                {
                    if (waveData.enemiesSpawnedThisWave >= waveData.numberOfEnemiesEachWave)
                    {
                        currentLoopState = GameplayLoopState.WaitingForEnemiesToClear;
                        return;
                    }

                    // Pull the next pre-planned enemy from our tactical queue
                    PoolGameObjectType nextEnemyType = waveSpawnQueue.Dequeue();
                    enemGO = enemySpawner.SpawnSpecificEnemy(nextEnemyType);

                    waveData.enemiesSpawnedThisWave++;
                    waveData.TotalAliveEnemies++;
                    waveData.timer = waveData.Cooldown;
                }
                break;
            case GameplayLoopState.WaitingForEnemiesToClear:
                if (waveData.TotalAliveEnemies <= 0)
                {
                    waveData.TotalAliveEnemies = 0;
                    waveData.timer = waveData.Cooldown;
                    if (waveData.Wave >= 10)
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
                waveData.timer -= Time.deltaTime;
                if (waveData.timer <= 0f)
                {
                    if (gameController.GetPlayer().stats.Health > 0)
                    {
                        if (waveData.HasBoss)
                        {
                            waveData.timer = waveData.Cooldown;

                            currentBoss = waveData.BossFights[UnityEngine.Random.Range(0, waveData.BossFights.Length)];
                            SpawnBoss(currentBoss, waveData.Difficulty);
                            waveData.enemiesSpawnedThisWave = 1;

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
                if (!waveData.BossBattleInitiated)
                {
                    if (gameController.CurrentGameState != GameState.GAMEOVER)
                    {
                         //gameController.GameOver();
                          currentLoopState = GameplayLoopState.Ended;
                          return;
                    }
                    waveData.timer = waveData.Cooldown;
                    currentLoopState = GameplayLoopState.RewardScreenActive;
                }
                break;
            case GameplayLoopState.RewardScreenActive:

                break;
            case GameplayLoopState.RewardClaimed:
                waveData.timer -= Time.deltaTime;
                if (waveData.timer <= 0f)
                {
                    waveData.timer = waveData.Cooldown;
                    currentLoopState = GameplayLoopState.HyperspaceTransition;
                }
                break;
            case GameplayLoopState.HyperspaceTransition:
                waveData.timer -= Time.deltaTime;
                if (waveData.timer <= 0f)
                {
                    currentLoopState = GameplayLoopState.PreWaveDelay; // Loop back for next wave
                }
                break;
        }
    }

    //======================================================================================================================================================
    public void NewWave()
    {
        waveData.HasBoss = false;
        waveData.enemiesSpawnedThisWave = 0; // Reset spawn counter per wave
        waveData.TotalAliveEnemies = 0;
        waveData.Wave++;

        if (waveData.Wave > 0 && waveData.Wave % 2 == 0)
        {
            waveData.availableEnemies++;
            waveData.Difficulty += 1;
            if (waveData.availableEnemies > waveData.enemyElements.Count)
            {
                waveData.availableEnemies = waveData.enemyElements.Count;
            }
        }

        if (waveData.Wave > 0 && waveData.Wave % 2 == 0)
        {
            waveData.HasBoss = true;
        }
        eventService.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.SURVIVE, value = waveData.Wave });

        GenerateWaveQueue();

        eventService.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.SURVIVE, value = waveData.Wave });

    }
    //=================================================================================
    private void GenerateWaveQueue()
    {
        waveSpawnQueue.Clear();
        waveEnemyQueList.Clear();
        if (waveData.enemyElements == null || waveData.enemyElements.Count == 0) return;

        int maxIndex = Mathf.Clamp(waveData.availableEnemies, 1, waveData.enemyElements.Count);

        for (int i = 0; i < waveData.numberOfEnemiesEachWave; i++)
        {
            PoolGameObjectType selectedType;

            // Rule: Early wave spawns favor scouts/weavers; later waves mix in heavy units
            if (waveData.Wave >= 3 && i == waveData.numberOfEnemiesEachWave - 1 && maxIndex >= 4)
            {
                // Put a heavy/area-denial unit at the very end of the queue
                selectedType = waveData.enemyElements[UnityEngine.Random.Range(3, maxIndex)];
            }
            else if (i % 2 == 0)
            {
                // Alternate between fast pressure units and pattern shooters
                selectedType = waveData.enemyElements[0]; // e.g., Fast Scout
            }
            else
            {
                selectedType = waveData.enemyElements[UnityEngine.Random.Range(0, maxIndex)];
            }

            waveEnemyQueList.Add(selectedType);
            waveSpawnQueue.Enqueue(selectedType);
        }
    }
    //=================================================================================
    public BossEnemy SpawnBoss(GameObject BossPrefab, int difficulty = 1)
    {
        GameObject currentBoss = GameObject.Instantiate(BossPrefab);
        currentBoss.name = BossPrefab.name;

        BossEnemy enemy = currentBoss.GetComponentInChildren<BossEnemy>();
        enemy.SetStats(difficulty);
        return enemy;
    }
    //=================================================================================
    public void SetLevel(int level)
    {
        waveData.Difficulty = level;
        enemySpawner.gameObjectTypeList = waveData.enemyElements;
        enemySpawner.LevelDifficulty = waveData.Difficulty;
    }
    //=================================================================================
    private void StartHyperspaceSequence()
    {
        currentLoopState = GameplayLoopState.HyperspaceTransition;
    }
    //=================================================================================
    public virtual void OnEnemyDiedHandled(EnemyDiedEvent enemyDied)
    {
        waveData.TotalAliveEnemies--;
        playerData.EnemyKilled++;
        if (enemyDied.WasBoss)
        {
            eventService.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.KILL, value = playerData.EnemyKilled });
        }
        else
        {
            eventService.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.BOUNTY, value = 1 });
        }
    }
    //=================================================================================
    public void OnEnemyEscapedCallback(EnemyEscapedEvent enemyEscaped)
    {
        playerData.EnemyEscaped++;
        waveData.TotalAliveEnemies--;
    }
}