using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class EnemyManager
{
    private static EnemyManager instance;

    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EnemyManager(GameController.Instance, SpawnEnemies.Instance);
            }
            return instance;
        }
    }
    public GameController gameController;
    public SpawnEnemies spawnEnemies;

    private float searchCountdown = 1f;
    private int availableEnemies;

    private static List<BaseEnemy> ListOfSpawnedEnemies = new List<BaseEnemy>();

    public EnemyManager(GameController gameController, SpawnEnemies spawnEnemies)
    {
        this.gameController = gameController;
        this.spawnEnemies = spawnEnemies;
    }

    public int NumberOfEnemies
    {
        get { return ListOfSpawnedEnemies.Count; }
    }

    public void SpawnBoss(GameObject bossPrefab, int difficulty)
    {
        gameController.NumberOfEnemies++;
        GameObject BossGO = GameObject.Instantiate(bossPrefab, bossPrefab.transform.localPosition, bossPrefab.transform.localRotation);

        int playerLevel = PlayerManager.GetPlayer().Level;
        BaseEnemy enemy = BossGO.GetComponentInChildren<BaseEnemy>();
        enemy.SetEnemyStats(difficulty);

        Add(enemy);
        gameController.EnemySpawnedInTotal++;
        BossGO.SetActive(true);
    }

    public void CreateEnemy(EnemyElement enemyElement, int LevelDifficulty)
    {
        gameController.NumberOfEnemies++;

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

        enemy.SetEnemyStats(LevelDifficulty);

        enemyElement.currentNumberInScene++;

        gameController.EnemySpawnedInTotal++;
    }

    public void CreateEnemy(EnemyElement enemyElement)
    {
        gameController.NumberOfEnemies++;

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

        enemy.SetEnemyStats(gameController.LevelDifficulty);

        enemyElement.currentNumberInScene++;

        gameController.EnemySpawnedInTotal++;
    }


    public static void Remove(BaseEnemy baseEnemy)
    {
        ListOfSpawnedEnemies.Remove(baseEnemy);
    }

    public static void Add(BaseEnemy baseEnemy)
    {
        ListOfSpawnedEnemies.Add(baseEnemy);
    }
}