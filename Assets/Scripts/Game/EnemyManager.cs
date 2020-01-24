using System.Collections.Generic;
using UnityEngine;

public class EnemyManager
{
    private static EnemyManager instance;

    public static EnemyManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            return null;
        }
    }

    private GameManager Gm;
    private GuiManager Gui;
    private ComboKillIndicator comboKillIndicator;

    private float searchCountdown = 1f;
    private int availableEnemies;


    //Variables for Spawning Enemies
    public static int EnemiesKilled;
    public static int EnemiesEscaped;
    public static int NumberOfEnemies;
    public static int MaxEnemiesToSpawn = 8;
    public static int EnemySpawnedInTotal = 0;

    [SerializeField] private GameObject[] BossPrefab;
    public List<BaseEnemy> ListOfSpawnedEnemies = new List<BaseEnemy>();
    private List<EnemyElement> ListOfEnemyToSpawn;

    private Player p;
    private PlayerWeaponSystem playerWeaponSystem;
    private DropController dropController;
    private GameObject BossGO;



    public EnemyManager(GameObject[] newBossPrefab)
    {
        instance = this;

        Init();
        BossPrefab = newBossPrefab;
        ListOfEnemyToSpawn = new List<EnemyElement>();
    }

    public static void Init()
    {
        PlayerManager.GetPlayer().GetWeaponSystem().IncreasePowerUp(0);
        EnemyManager.NumberOfEnemies = 0;
        SpawnEnemies.CurrentEnemyKilled = 0;
        EnemyManager.EnemiesEscaped = 0;
        SpawnEnemies.EnemyKilled = 0;
        EnemySpawnedInTotal = 0;

        GameEventSystem.OnEnemyDeathHandled += EnemyDied;
        GameEventSystem.OnEnemyDeathHandled += EnemyEscaped;
    }

    public void CreateEnemy(EnemyElement enemyElement,int LevelDifficulty)
    {
        EnemyManager.NumberOfEnemies++;

        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(Constants.m_XMin, Constants.m_XMax), -50, Constants.m_ZMax);

        GameObject enemGO = PoolManager.Instance.GetObjectFromPool(enemyElement.gameObjectType);

        BaseEnemy enemy = enemGO.GetComponent<BaseEnemy>();
        FollowPathAI followPathAI = enemGO.GetComponent<FollowPathAI>();
        if (followPathAI == null)
        {
            enemGO.transform.position = spawnPos;
            enemGO.transform.rotation = Quaternion.identity;
        }
        else
        {
            followPathAI.GeneratePath();
        }

        enemy.enemyElement = enemyElement;

        enemy.SetEnemyStats(LevelDifficulty, EnemyDied, EnemyEscaped);

        enemyElement.currentNumberInScene++;

        EnemySpawnedInTotal++;
    
    
    }

    public void CreateEnemy(EnemyElement enemyElement)
    {
        EnemyManager.NumberOfEnemies++;

        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(Constants.m_XMin, Constants.m_XMax), -50, Constants.m_ZMax);

        GameObject enemGO = PoolManager.Instance.GetObjectFromPool(enemyElement.gameObjectType);

        BaseEnemy enemy = enemGO.GetComponent<BaseEnemy>();
        FollowPathAI followPathAI = enemGO.GetComponent<FollowPathAI>();
        if (followPathAI == null)
        {
            enemGO.transform.position = spawnPos;
            enemGO.transform.rotation = Quaternion.identity;
        }
        else
        {
            followPathAI.GeneratePath();
        }

        enemy.enemyElement = enemyElement;

        enemy.SetEnemyStats(SpawnEnemies.LevelDifficulty, EnemyDied, EnemyEscaped);

        enemyElement.currentNumberInScene++;

        EnemySpawnedInTotal++;
    }


    public void SpawnBoss(int difficulty)
    {
        EnemyManager.NumberOfEnemies++;
        GameObject bossprefab = BossPrefab[UnityEngine.Random.Range(0, BossPrefab.Length)];
        BossGO = GameObject.Instantiate(bossprefab, bossprefab.transform.localPosition, bossprefab.transform.localRotation);

        int playerLevel = PlayerManager.GetPlayer().Level;
        BaseEnemy enemy = BossGO.GetComponentInChildren<BaseEnemy>();
        enemy.SetEnemyStats(SpawnEnemies.LevelDifficulty, EnemyDied, EnemyEscaped);

        EnemyManager.Instance.ListOfSpawnedEnemies.Add(enemy);

        EnemyManager.EnemySpawnedInTotal++;
        BossGO.SetActive(true);
    }
    public static void EnemyEscaped(BaseEnemy enemy)
    {
        NumberOfEnemies--;
        EnemiesEscaped++;
        enemy.enemyElement.currentNumberInScene--;
        instance.ListOfSpawnedEnemies.Remove((BaseEnemy)enemy);
    }

    public static void EnemyDied(BaseEnemy enemy)
    {
        Vector3 enemyPos = enemy.transform.position;

        if (EnemyManager.instance.BossGO != null)
        {
            if (GuiManager.Instance)
            {
                if (!SpawnEnemies.Instance.survival)
                {
                    GuiManager.Instance.GameOver();
                }
                else
                {
                    SpawnEnemies.Instance.bossWave = false;
                }
            }
        }
     
        SpawnEnemies.EnemyKilled++;
        SpawnEnemies.CurrentEnemyKilled++;
        PlayerData playerData = DataController.GetPlayerData();
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Kill);
        if (objectiveData != null)
            objectiveData.UpdateProgress(SpawnEnemies.CurrentEnemyKilled);

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
        SpawnEnemies.Score += score;
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

        instance.ListOfSpawnedEnemies.Remove((BaseEnemy)enemy);

        GameObject.Destroy(EnemyManager.instance.BossGO,2f);

    }

    public static bool CheckIfWeReachedSpawnLimit()
    {
        return EnemyManager.NumberOfEnemies < EnemyManager.MaxEnemiesToSpawn;
    }

    public static bool CheckIfWeKilledEnoughEnemiesToProgress()
    {
        return (SpawnEnemies.CurrentEnemyKilled + EnemyManager.EnemiesEscaped) >= EnemyManager.MaxEnemiesToSpawn;
    }

    public static bool CheckIfCurrentEnemiesAreMoreThanMax()
    {
        return EnemyManager.NumberOfEnemies >= EnemyManager.MaxEnemiesToSpawn;
    }

    public static bool CheckIfCurrentAreLessThanMax()
    {
        return EnemyManager.NumberOfEnemies <= EnemyManager.MaxEnemiesToSpawn;
    }
}