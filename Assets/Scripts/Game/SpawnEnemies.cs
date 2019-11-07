using System;
using System.Collections;
using System.Collections.Generic;
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

    private EnemyManager enemyManager;
    private Coroutine SpawnerCoroutine;
    private SpawnerState spawnerState;
    private float delay;
    public bool bossWave;
    private int Wave = 0;
    private float countdown;
    private float minDelay = .5f;
    private float maxDelay = 1;
    private int AvailableEnemies;
    public bool spawnReady;

    [SerializeField] public bool survival = false;

    [Range(1, 21)]
    private int LevelDifficuilty = 1;

    private int LevelIncreaseThreshold = 2;


    [SerializeField] private float countdownDelay;
    [SerializeField] private bool BossStage;
    [SerializeField] private GameObject[] BossPrefab = null;
    [SerializeField] private int maxWave;
    [SerializeField] private AudioClip GameOverClip;
    [SerializeField] private List<EnemyElement> ListOfEnemyToSpawn = new List<EnemyElement>();

    public int AvailableEnemiesIndex = 1;
    public static int LevelDifficulty { get; private set; }
    public static bool GameOver { get; set; }
    public static float Score { get; set; }
    public static int WaveSurvived { get; set; }
    public static int EnemyKilled { get; set; }
    public static int CurrentEnemyKilled { get; set; }
    public static int CoinDropInTotal { get; set; }
    public static int counsEarnInGame { get; set; }

    public int MaxWave
    {
        get
        {
            return maxWave;
        }
        set
        {
            maxWave = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!survival)
        {
            MissionCollection missionCollection = DataController.GetMissionCollection();
            Mission mission = missionCollection.GetMission(GameManager.LevelSelected);
            LevelDifficulty = mission.Level;
        }
        else
        {
            LevelDifficulty = 1;
        }

        EnemyManager enemyManager = new EnemyManager(BossPrefab);
        PlayerManager.GetPlayer().GetWeaponSystem().ResetWeaponPowerUPCollected();
        GameStart();
    }

    public void GameStart()
    {
        GameOver = false;
        countdown = countdownDelay;
        GameManager.instance.SetState(GameStates.Game);

        if (survival)
        {
            AvailableEnemiesIndex = 1;
        }

        if (SpawnerCoroutine == null)
            SpawnerCoroutine = StartCoroutine(Spawn());



    }

    private void Update()
    {
        //If Music is Done Choose something new to play.
        if (GameManager.instance.GetCurrentState() != GameStates.GameOver)
        {

            AudioManager.PlayRandomMusic();
        }


        if (spawnerState != SpawnerState.Idle)
        {
            Spawning();
        }


    }

    public void Spawning()
    {
        if (GameOver == false)
        {
            //if Number of Enemies that are spawn is more that Max don't spawn anymore
            if (EnemyManager.CheckIfCurrentEnemiesAreMoreThanMax())
            {
                spawnerState = SpawnerState.wait;
            }
        }
        else if (GameOver == true)
        {
            StopCoroutine(Spawn());
            spawnerState = SpawnerState.stopped;
        }
    }

    private IEnumerator Spawn()
    {
        while (GameOver == false)
        {
            switch (spawnerState)
            {
                case SpawnerState.stopped:

                    if (EnemyManager.CheckIfCurrentAreLessThanMax())
                    {
                        spawnerState = SpawnerState.wait;
                    }
                    break;

                case SpawnerState.wait:

                    if (bossWave)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(2.0f);
                    if (Wave == 0 || EnemyManager.CheckIfWeKilledEnoughEnemiesToProgress())
                    {
                        WaveSurvived = Wave;

                        PlayerData playerData = DataController.GetPlayerData();
                        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.survive);
                        if (objectiveData != null)
                        {
                            objectiveData.UpdateProgress(WaveSurvived);
                        }

                        Wave++;

                        if (survival)
                        {
                            if (Wave % LevelIncreaseThreshold == 0)
                            {
                                LevelDifficuilty++;
                            }

                            if (Wave % 8 == 0)
                            {
                                AvailableEnemiesIndex++;

                                AvailableEnemiesIndex = Mathf.Clamp(AvailableEnemiesIndex, 1, ListOfEnemyToSpawn.Count);
                            }
                        }

                        //AvailableEnemiesIndex++;
                        // AvailableEnemiesIndex = Mathf.Clamp(AvailableEnemiesIndex, 0, ListOfEnemyToSpawn.Count);

                        GuiManager.CountdownVisibility(true);

                        SpawnEnemies.CurrentEnemyKilled = 0;
                        EnemyManager.EnemiesEscaped = 0;
                        if (survival)
                        {
                            if (Wave % 2 == 0)
                            {
                                bossWave = true;
                                AudioManager.PlaySound("Danger", 3);
                                string[] transmitions = { "There is something Big Coming on your way", "Be Careful" };
                                GuiManager.PlayTrasmition(transmitions);

                                while (GuiManager.IsTrasnmiting())
                                {
                                    GuiManager.CountdownVisibility(false);
                                    yield return new WaitForSeconds(.1f);
                                }

                                while (EnemyManager.NumberOfEnemies > 0)
                                {
                                    yield return null;
                                }

                                countdown = countdownDelay;
                            }
                        }
                        else
                        {
                            if (Wave >= MaxWave)
                            {
                                if (BossStage)
                                {
                                    bossWave = true;
                                    AudioManager.PlaySound("Danger", 3);
                                    string[] transmitions = { "There is something Big Coming on your way", "Be Careful" };
                                    GuiManager.PlayTrasmition(transmitions);

                                    while (GuiManager.IsTrasnmiting())
                                    {
                                        GuiManager.CountdownVisibility(false);
                                        yield return new WaitForSeconds(.1f);
                                    }

                                    while (EnemyManager.NumberOfEnemies > 0)
                                    {
                                        yield return null;
                                    }

                                    countdown = countdownDelay;
                                }
                                else
                                {
                                    while (EnemyManager.NumberOfEnemies > 0)
                                    {
                                        yield return null;
                                    }

                                    GameOver = true;
                                }
                            }
                        }

                        if (!survival)
                        {
                            if (Wave == 1 && !bossWave)
                            {
                                yield return new WaitForSeconds(1.5f);

                                string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
                                GuiManager.PlayTrasmition(transmitions);

                            }
                        }
                        else
                        {
                            if (Wave == 1 && !bossWave)
                            {
                                yield return new WaitForSeconds(1.5f);
                                string[] transmitions = { "Survive", "Good Luck!" };

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



                    if (GameOver)
                    {
                        yield return new WaitForSeconds(.5f);

                        GuiManager.Instance.GameOver();

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
                    if (EnemyManager.CheckIfWeReachedSpawnLimit())
                    {
                        if (bossWave == true)
                        {
                            AudioManager.SetMusic("Boss");
                            EnemyManager.Instance.SpawnBoss(LevelDifficulty);
                        }
                        else if (bossWave == false)
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
            if (survival)
            {
                EnemyManager.Instance.CreateEnemy(enemyElement, LevelDifficuilty);
            }
            else
            {
                EnemyManager.Instance.CreateEnemy(enemyElement);
            }
        }

    }

    public static void SetLevelDifficuilty(int Level)
    {
        LevelDifficulty = Level;
    }
}