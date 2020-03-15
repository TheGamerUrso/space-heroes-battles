using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public bool IsGameOver;

    private EnemyManager enemyManager;
    private SpawnEnemies spawnEnemies;
    private BossBattleSystem bossBattleSystem;

    private GameManager Gm;
    private GuiManager Gui;
    private ComboKillIndicator comboKillIndicator;

    private Player p;
    private PlayerWeaponSystem playerWeaponSystem;
    private DropController dropController;
    private GameObject BossGO;
    public bool BossStage;

    public bool bossWave;
    public int Wave = 0;

    [Range(1, 21)]
    public int LevelDifficuilty = 1;
    public int LevelIncreaseThreshold = 2;
    public bool survivalMode = false;


    //Variables for Spawning Enemies
    public int EnemiesKilled;
    public int EnemiesEscaped;
    public int NumberOfEnemies;
    public int MaxEnemiesToSpawn = 8;
    public int EnemySpawnedInTotal = 0;

    public int LevelDifficulty { get; private set; }
    public float Score { get; set; }
    public int WaveSurvived { get; set; }
    public int EnemyKilled { get; set; }
    public int CurrentEnemyKilled { get; set; }
    public int CoinDropInTotal { get; set; }
    public int counsEarnInGame { get; set; }

    [SerializeField] private int maxWave;
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

    [SerializeField] private AudioClip GameOverClip;

    void Start()
    {
        bossBattleSystem = GameObject.FindObjectOfType<BossBattleSystem>();
        IsGameOver = false;

        Application.targetFrameRate = 60;
        AudioManager.PlayRandomMusic(true);
        //EnemyManager enemyManager = new EnemyManager(BossPrefab);

        //Initial Enemy Manager
        PlayerManager.GetPlayer().GetWeaponSystem().IncreasePowerUp(0);
        NumberOfEnemies = 0;
        EnemiesEscaped = 0;
        EnemySpawnedInTotal = 0;

        GameEventSystem.OnEnemyDeath += EnemyDied;
        GameEventSystem.OnEnemyDeath += EnemyEscaped;
        PlayerManager.GetPlayer().GetWeaponSystem().ResetWeaponPowerUPCollected();

        if (!survivalMode)
        {
            MissionCollection missionCollection = DataController.GetMissionCollection();
            Mission mission = missionCollection.GetMission(GameManager.LevelSelected);
            LevelDifficulty = mission.Level;
        }
        else
        {
            LevelDifficulty = 1;
        }
    }

    void Update()
    {
        //If Music is Done Choose something new to play.    
        if (AudioManager.Instance)
        {
            if (!IsGameOver && AudioManager.Instance.MusicIsDone())
            {
                AudioManager.PlayRandomMusic();
            }
        }
    }

    public void EnemyEscaped(string id,BaseEnemy enemy)
    {
        NumberOfEnemies--;
        EnemiesEscaped++;
        enemy.enemyElement.currentNumberInScene--;
        EnemyManager.Remove((BaseEnemy)enemy);
    }

    public void EnemyDied(string id,BaseEnemy enemy)
    {
        Vector3 enemyPos = enemy.transform.position;
        BaseBossEnemy boss = enemy.GetComponent<BaseBossEnemy>();
        if (boss != null)
        {
            if (GuiManager.Instance)
            {
                if (!survivalMode)
                {
                    GuiManager.Instance.GameOver();
                }
                else
                {
                    bossWave = false;
                }
            }
        }

        EnemyKilled++;
        CurrentEnemyKilled++;

        PlayerData playerData = DataController.GetPlayerData();
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Kill);

        if (objectiveData != null)
            objectiveData.UpdateProgress(CurrentEnemyKilled);

        NumberOfEnemies--;
        enemy.enemyElement.currentNumberInScene--;


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
        GuiManager.Instance.UpdateScore(score);

        GuiManager.Instance.CreateFloatingText(string.Format("{0}", score), enemyPos);

        //Update Player Attributes

        if (PlayerManager.GetPlayer() == null)
        {
            return;
        }

        Player p = PlayerManager.GetPlayer();
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

        EnemyManager.Remove((BaseEnemy)enemy);

        GameObject.Destroy(BossGO, 2f); 

    }

    public bool CheckIfWeReachedSpawnLimit()
    {
        return NumberOfEnemies < MaxEnemiesToSpawn;
    }

    public bool CheckIfWeKilledEnoughEnemiesToProgress()
    {
        return (CurrentEnemyKilled + EnemiesEscaped) >= MaxEnemiesToSpawn;
    }

    public bool CheckIfCurrentEnemiesAreMoreThanMax()
    {
        return NumberOfEnemies >= MaxEnemiesToSpawn;
    }

    public bool CheckIfCurrentAreLessThanMax()
    {
        return NumberOfEnemies <= MaxEnemiesToSpawn;
    }

    public void SetLevelDifficuilty(int Level)
    {
        LevelDifficulty = Level;
    }
}
