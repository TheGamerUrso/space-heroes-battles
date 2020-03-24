using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum LevelMode
{
    Normal, Boss, GameOver
}

public class GameController : Singleton<GameController>
{
    public Action<int, int, int> GameStatsChanged;
    private GameObject playerShip;
    public static bool IsGameOver;
    public static bool useSloMo;

    public LevelMode levelMode = LevelMode.Normal;
    public bool BossFight;

    public int Wave;
    public int MaxWave;

    public int enemyKilled;
    public int enemyEscaped;

    public int TotalEnemies;

    [SerializeField] private int numberOfEnemiesEachWave;
    public int NumberOfEnemiesEachWave
    {
        get { return numberOfEnemiesEachWave; }
    }

    public int NumberOfEnemies;
    public int LevelDifficulty { get; set; }
    public float Score { get; set; }
    public int WaveSurvived { get; set; }
    public int EnemyKilled { get; set; }
    public int CurrentEnemyKilled { get; set; }
    public int CoinDropInTotal { get; set; }
    public int counsEarnInGame { get; set; }
    public int EnemySpawnedInTotal { get; set; }

    private float slowMo;

    private float delayTheSlowMoEffectTimer;

    public List<IDestroyable> Enemies = new List<IDestroyable>();

    public List<IEndGameObserver> gameObservers = new List<IEndGameObserver>();

    public void AddObserver(IEndGameObserver observer)
    {
        gameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver destroyable)
    {
        gameObservers.Remove(destroyable);
    }

    public void AddEnemy(IDestroyable destroyable)
    {
        Enemies.Add(destroyable);
        BaseEnemy baseEnemy = destroyable as BaseEnemy;
        if (baseEnemy != null)
        {
            baseEnemy.EnemyDied += EnemyDied;
            baseEnemy.EnemyGotHit += EnemyGotHit;
            baseEnemy.EnemyEscaped += EnemyEscaped;
        }
    }

    public void RemoveEnemy(IDestroyable destroyable)
    {
        Enemies.Add(destroyable);
        BaseEnemy baseEnemy = destroyable as BaseEnemy;
        if (baseEnemy != null)
        {
            baseEnemy.EnemyDied -= EnemyDied;
            baseEnemy.EnemyGotHit -= EnemyGotHit;
            baseEnemy.EnemyEscaped -= EnemyEscaped;
        }
    }

    public void NotifyObservers()
    {
        foreach (IEndGameObserver enemy in gameObservers.ToArray())
        {
            enemy.GameOver();
        }
    }


    [SerializeField] private List<GameObject> EnemySpawned = new List<GameObject>();

    protected override void OnAwake()
    {
        Scene bootScene = SceneManager.GetSceneByName("boot");
        if (!bootScene.isLoaded)
        {
            SceneManager.LoadScene("boot", LoadSceneMode.Additive);
        }
    }

    void Start()
    {

        IsGameOver = false;

        Application.targetFrameRate = 60;
        AudioManager.PlayRandomMusic(true);


        MissionCollection missionCollection = DataController.GetMissionCollection();
        Mission mission = missionCollection.GetMission(GameManager.LevelSelected);
        LevelDifficulty = mission.Level;

        delayTheSlowMoEffectTimer = 4;


        TotalEnemies = MaxWave * numberOfEnemiesEachWave;


        GameStatsChanged?.Invoke(Wave, MaxWave, TotalEnemies);

        SpawnEnemies.Instance.OnSpawnEnemy += (x) =>
        {
            TotalEnemies--; GameStatsChanged?.Invoke(Wave, MaxWave, TotalEnemies);
            EnemySpawned.Add(x);
        };



        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = GameManager.CurrentHeroChoosen;
            playerShip = PlayerManager.CreatePlayer(shipSelected);
        }

        playerShip.GetComponent<PlayerShip>().PlayerShipDeath += GameOver;

        SpawnEnemies.Instance.StartGame();
    }

    public void ToggleSlowMo(bool value)
    {
        useSloMo = value;
        if (value == false)
        {
            Time.timeScale = 1.0f;
        }
    }

    public void NewWave()
    {
        if (levelMode == LevelMode.Normal)
        {
            Wave++;
            // string[] transmitions = { "Wave " + GameController.Instance.Wave, "Enemies Approaching", "Defeat them", "Good Luck" };
            // GuiManager.PlayTrasmition(transmitions);
            GameStatsChanged?.Invoke(Wave, MaxWave, TotalEnemies);
        }
    
        if (Wave >= MaxWave)
        {
            levelMode = LevelMode.Boss;
        }
    }

    private void Update()
    {
        if (AudioManager.Instance)
        {
            if (!IsGameOver && AudioManager.Instance.MusicIsDone())
            {
                AudioManager.PlayRandomMusic();
            }
        }

        SlowMoEffect();

        if (EnemySpawned.Count <= 0 && TotalEnemies <= 0)
        {
            if (!BossFight)
            {
                Win();
            }
        }



    }
    IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(4.0f);
        NotifyObservers();
        GuiManager.Instance.GameOver();

    }
    IEnumerator DelayWinScreen()
    {
        yield return new WaitForSeconds(4.0f);
        NotifyObservers();
        GuiManager.Instance.Win();

    }

    public void SlowMoEffect()
    {
#if UNITY_ANDROID
        if (useSloMo && !GameManager.Paused)
        {
            if (Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                //     slowMo = 1;
            }
            else
            {
                //       slowMo = .3f;

            }
            // Time.timeScale = slowMo;
        }
#endif
    }


    public void Win()
    {
        if (!IsGameOver)
        {
            IsGameOver = true;
            StartCoroutine(DelayWinScreen());
        }
    }
    public void GameOver()
    {
        if (IsGameOver == false)
        {
            IsGameOver = true;
            StartCoroutine(DelayGameOver());
            NotifyObservers();
        }
    }

    public void EnemyGotHit(string id, BaseEnemy enemy)
    {

    }

    public void EnemyEscaped(string id, BaseEnemy enemy)
    {
        EnemySpawned.Remove(enemy.gameObject);
        enemyEscaped++;
        GameStatsChanged?.Invoke(Wave, MaxWave, TotalEnemies);
        enemy.enemyElement.currentNumberInScene--;
    }

    public void EnemyDied(string id, BaseEnemy enemy)
    {
        if (enemy.GetComponent<BossBattleSystem>() && BossFight)
        {
            levelMode = LevelMode.GameOver;
            Win();
        }

        EnemySpawned.Remove(enemy.gameObject);
        Vector3 enemyPos = enemy.transform.position;

        enemyKilled++;
        GameStatsChanged?.Invoke(Wave, MaxWave, TotalEnemies);
        enemy.enemyElement.currentNumberInScene--;

        PlayerData playerData = DataController.GetPlayerData();
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Kill);

        if (objectiveData != null)
            objectiveData.UpdateProgress(CurrentEnemyKilled);

        //Update ComboKillIndicator
        if (ComboKillIndicator.instance)
            ComboKillIndicator.instance.ConfirmKill();

        int multiplayer = 0;
        if (ComboKillIndicator.instance)
        {
            multiplayer = ComboKillIndicator.instance.GetMultiplier();
        }

        //Update Score
        int score = multiplayer * enemy.m_ValueOfEnemy;
        Score += score;

        if (GuiManager.Instance)
        {
            GuiManager.Instance.UpdateScore(score);
            GuiManager.CreateFloatingText(string.Format("{0}", score), enemyPos);
        }

        //Update Player Attributes

        if (PlayerManager.GetPlayer() == null)
        {
            return;
        }

        PlayerShip p = PlayerManager.GetPlayer();
        PlayerWeaponSystem playerWeaponSystem = p.GetWeaponSystem();


        int PlayerLevel = p.Level;
        int EnemyLevel = enemy.Level;
        int levelDiffrence = PlayerLevel / EnemyLevel;

        if (levelDiffrence == 0)
        {
            levelDiffrence = 1;
        }

        // float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
        float XPEarned = 5 / levelDiffrence;
        p.GetLevelSystem().AddXP((int)XPEarned);
        Debug.Log(string.Format("exp = {0}\n", XPEarned));


        playerWeaponSystem.IncreasePowerUp(.1f);

        //Drop Item
        DropController.PickRandomDropItem(enemy.transform);


        if (enemy.GetComponent<BossBattleSystem>())
        {
            return;
        }
    }


    /**
 

    private EnemyManager enemyManager;
    private SpawnEnemies spawnE;
    private BossBattleSystem bossBattleSystem;

    private GameManager Gm;
    private GuiManager Gui;
    private ComboKillIndicator comboKillIndicator;

    private Player p;
    private PlayerWeaponSystem playerWeaponSystem;
    private DropController dropController;



    [Range(1, 21)]
    public int LevelDifficuilty = 1;
    public int LevelIncreaseThreshold = 2;

    public bool HasBoss;
    public bool survivalMode = false;
    public bool BossStage;

    public int Wave = 0;
    [SerializeField] private int maxWave;
    public bool bossWave;

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

    public int enemyKilled;
    public int enemyEscaped;

    //Variables for Spawning Enemies
    public int EnemiesKilled;
    public int EnemiesEscaped;
    public int NumberOfEnemies;
    public int MaxEnemiesToSpawn = 8;
    public int EnemySpawnedInTotal = 0;



    [SerializeField] private AudioClip GameOverClip;

    void Start()
    {  
        IsGameOver = false;

        Application.targetFrameRate = 60;
        AudioManager.PlayRandomMusic(true);
        //EnemyManager enemyManager = new EnemyManager(BossPrefab);

        //Initial Enemy Manager
        PlayerManager.GetPlayer().GetWeaponSystem().IncreasePowerUp(0);

        GameEventSystem.OnEnemyDeath += EnemyDied;
        GameEventSystem.OnEnemyDeath += EnemyEscaped;
        PlayerManager.GetPlayer().GetWeaponSystem().ResetWeaponPowerUPCollected();



        MissionCollection missionCollection = DataController.GetMissionCollection();
        Mission mission = missionCollection.GetMission(GameManager.LevelSelected);
        LevelDifficulty = mission.Level;

        SpawnEnemies.Instance.SetupSpawnEnemies(this);
    }

        
    */

    public void SetLevelDifficuilty(int Level)
    {
        LevelDifficulty = Level;
    }

}
