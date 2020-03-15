using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;

[Serializable]
public class EnemyElement
{
    public string Name;
    public PoolGameObjectType gameObjectType;
    public GameObject Prefab;
    public int currentNumberInScene;
    public int MaxNumberInScene;
}
public enum SpawnerState
{
    stopped, wait, spawning, Idle
}

public class SpawnEnemies : MonoBehaviour
{
    public static SpawnEnemies Instance;
    public GameController gameController;
    public BossBattleSystem bossBattleSystem;

    private Coroutine SpawnerCoroutine;
    private SpawnerState spawnerState;
    private float delay;

    private float countdown;
    private float minDelay = .3f;
    private float maxDelay = .5f;

    public bool spawnReady;

    [SerializeField] private float countdownDelay;
 
    [SerializeField] private List<EnemyElement> ListOfEnemyToSpawn = new List<EnemyElement>();

    [SerializeField] private int AvailableEnemiesIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameController = GameController.Instance;
        if (gameController)
        {
            bool SurvivalMode = gameController.survivalMode;
            int LevelDifficulty = gameController.LevelDifficulty;

            countdown = countdownDelay;

            new EnemyManager(gameController, this);

            if (SpawnerCoroutine == null)
                SpawnerCoroutine = StartCoroutine(Spawn());
        }
    }


    private void Update()
    {
        if (spawnerState != SpawnerState.Idle)
        {
            Spawning();
        }
    }

    public void Spawning()
    {
        if (gameController)
        {
            if (gameController.IsGameOver == false)
            {
                //if Number of Enemies that are spawn is more that Max don't spawn anymore
                if (gameController.CheckIfCurrentEnemiesAreMoreThanMax())
                {
                    spawnerState = SpawnerState.wait;
                }
            }
            else if (gameController.IsGameOver == true)
            {
                StopCoroutine(Spawn());
                spawnerState = SpawnerState.stopped;
            }
        }
    }

    private IEnumerator Spawn()
    {
        while (gameController.IsGameOver == false)
        {
            switch (spawnerState)
            {
                case SpawnerState.stopped:

                    if (gameController.CheckIfCurrentAreLessThanMax())
                    {
                        spawnerState = SpawnerState.wait;
                    }
                    break;

                case SpawnerState.wait:

                    if (gameController.BossStage)
                    {
                        break;
                    }

                    yield return new WaitForSeconds(2.0f);
                    if (gameController.Wave == 0 || gameController.CheckIfWeKilledEnoughEnemiesToProgress())
                    {
                        gameController.WaveSurvived = gameController.Wave;
                        PlayerData playerData = DataController.GetPlayerData();
                        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.survive);

                        if (objectiveData != null)
                        {
                            objectiveData.UpdateProgress(gameController.WaveSurvived);
                        }

                        gameController.Wave++;

                        if (gameController.survivalMode)
                        {
                            if (gameController.Wave % gameController.LevelIncreaseThreshold == 0)
                            {
                                gameController.LevelDifficuilty++;
                            }

                            if (gameController.Wave % 8 == 0)
                            {
                                AvailableEnemiesIndex++;

                                AvailableEnemiesIndex = Mathf.Clamp(AvailableEnemiesIndex, 1, ListOfEnemyToSpawn.Count);
                            }
                        }

                        //AvailableEnemiesIndex++;
                        // AvailableEnemiesIndex = Mathf.Clamp(AvailableEnemiesIndex, 0, ListOfEnemyToSpawn.Count);

                        GuiManager.CountdownVisibility(true);

                        gameController.CurrentEnemyKilled = 0;
                        gameController.EnemiesEscaped = 0;
                        if (gameController.survivalMode)
                        {
                            if (gameController.Wave % 2 == 0 && !gameController.bossWave)
                            {
                                gameController.bossWave = true;
                                bossBattleSystem.ShowBossFightWarning();

                                while (GuiManager.IsTrasnmiting())
                                {
                                    GuiManager.CountdownVisibility(false);
                                    yield return new WaitForSeconds(.1f);
                                }

                                while (gameController.NumberOfEnemies > 0)
                                {
                                    yield return null;
                                }

                                countdown = countdownDelay;
                            }
                        }
                        else
                        {
                            if (gameController.Wave >= gameController.MaxWave)
                            {
                                if (!gameController.bossWave)
                                {
                                    gameController.bossWave = true;

                                    bossBattleSystem.ShowBossFightWarning();
                                   
                                    while (GuiManager.IsTrasnmiting())
                                    {
                                        yield return new WaitForSeconds(.1f);
                                    }

                                    while (gameController.NumberOfEnemies > 0)
                                    {
                                        yield return null;
                                    }

                                    countdown = countdownDelay;
                                }
                                else
                                {
                                    while (gameController.BossStage)
                                    {
                                        yield return null;
                                    }
                                }
                            }
                        }

                        if (!gameController.survivalMode)
                        {
                            if (gameController.Wave == 1 && !gameController.bossWave)
                            {
                                yield return new WaitForSeconds(1.5f);

                                string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
                                GuiManager.PlayTrasmition(transmitions);
                            }
                        }
                        else
                        {
                            if (gameController.Wave == 1 && !gameController.bossWave)
                            {
                                yield return new WaitForSeconds(1.5f);
                                string[] transmitions = { "Wave" + gameController.Wave, "Level Difficulty " + gameController.LevelDifficuilty, " Ready!" , "GO" };

                                GuiManager.PlayTrasmition(transmitions);
                            }
                        }

                        while (GuiManager.IsTrasnmiting())
                        {
                            GuiManager.CountdownVisibility(false);
                            yield return null;
                        }

                        while (countdown > 0)
                        {
                            countdown -= Time.deltaTime;

                            GuiManager.CountdownVisibility(false);
                            GuiManager.Countdown(countdown);

                            yield return null;
                        }

                        // countdown = countdownDelay;
                        GuiManager.CountdownVisibility(false);
                    }

                    if (gameController.IsGameOver)
                    {
                        yield return new WaitForSeconds(.5f);
                        spawnerState = SpawnerState.Idle;
                    }
                    else
                    {
                        spawnerState = SpawnerState.spawning;
                    }
                    break;

                case SpawnerState.spawning:

                    delay = UnityEngine.Random.Range(minDelay, maxDelay);

                    yield return new WaitForSeconds(delay);
                    if (gameController.CheckIfWeReachedSpawnLimit())
                    {
                        if (gameController.bossWave && !gameController.BossStage)
                        {
                            bossBattleSystem.StartBossFight();
                        }
                        else if (gameController.bossWave == false)
                        {
                            spawnReady = true;
                            while (spawnReady)
                            {
                                int randomEnemy = UnityEngine.Random.Range(0, AvailableEnemiesIndex);

                                int random = UnityEngine.Random.Range(1, 2);

                                if (randomEnemy == 0)
                                {
                                    random = UnityEngine.Random.Range(5, 8);
                                }

                                // Debug.Log("Spawn " + random + " Of Enemies");
                                for (int i = 0; i < random; i++)
                                {
                                    SpawnEnemy(randomEnemy);
                                    yield return new WaitForSeconds(.5f);
                                }

                                spawnReady = false;

                                yield return null;
                            }
                        }
                    }

                    spawnerState = SpawnerState.wait;
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }

    public void SpawnEnemy(int randomEnemy)
    {
        EnemyElement enemyElement = ListOfEnemyToSpawn[randomEnemy];
        if (enemyElement.currentNumberInScene < enemyElement.MaxNumberInScene)
        {
            if (gameController.survivalMode)
            {
                EnemyManager.Instance.CreateEnemy(
                    enemyElement, gameController.LevelDifficuilty);
            }
            else
            {
                EnemyManager.Instance.CreateEnemy(enemyElement);
            }
        }

    }
}