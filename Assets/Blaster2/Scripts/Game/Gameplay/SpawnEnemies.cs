using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class EnemyElement
{
    public string Name;
    public PoolGameObjectType gameObjectType;
    public GameObject Prefab;
    public int currentNumberInScene;
    public int MaxNumberInScene;
    public int presentage;
}

public class SpawnEnemies
{
    private static Vector3 previousPos;

    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    private int previousIndex = 0;

    public SpawnEnemies(List<SpawnPoint> spawnPoints)
    {
        this.spawnPoints = spawnPoints;
    }

    public static BaseBossEnemy SpawnBoss(GameObject BossPrefab, int LevelDifficulty = 1)
    {
        AudioManager.Instance.PlayMusicById("Boss", true);
        GameObject currentBoss = GameObject.Instantiate(BossPrefab);
        currentBoss.name = BossPrefab.name;

        BaseBossEnemy enemy = currentBoss.GetComponentInChildren<BaseBossEnemy>();
        enemy.Id = currentBoss.name;
        enemy.SetStats(LevelDifficulty);

        BaseGameMode.Instance.spawnInfo.TotalEnemies++;
        return enemy;
    }

    IEnumerator EnableSpawnPointAgain(SpawnPoint spawnPoint)
    {
        yield return new WaitForSeconds(2.0f);
        spawnPoint.used = false;
    }

    public GameObject SpawnEnemyElement(EnemyElement enemyElement, int LevelDifficulty = 1)
    {
        enemyElement.currentNumberInScene++;

        SpawnPoint[] tempList = spawnPoints.Where(x => x.used == false).ToArray();

        int spawnIndex = UnityEngine.Random.Range(0, tempList.Length);

        //spawnPoints[previousIndex].used = false;

        GameController.Instance.StartCoroutine(EnableSpawnPointAgain(spawnPoints[previousIndex]));

        Vector3 spawnPos = tempList[spawnIndex].spawnPoint.transform.position;

        previousIndex = spawnIndex;

        spawnPoints[spawnIndex].used = true;

        //spawnPos = new Vector3(UnityEngine.Random.Range(Constants.m_XMin, Constants.m_XMax), 0, Constants.m_ZMax);

        GameObject enemGO = PoolManager.Instance.GetObjectFromPool(enemyElement.gameObjectType);

        BaseEnemy enemy = enemGO.GetComponent<BaseEnemy>();
        enemy.Id = enemyElement.Name + "_" + enemyElement.currentNumberInScene;
        FollowPathAI followPathAI = enemGO.GetComponent<FollowPathAI>();

        enemGO.SetActive(true);

        if (followPathAI == null)
        {
            enemGO.transform.position = spawnPos;
            enemGO.transform.rotation = Quaternion.LookRotation(Vector3.back);
        }
        else
        {
            enemGO.transform.position = spawnPos;
            followPathAI.GeneratePath();
        }

        enemy.enemyElement = enemyElement;

        enemy.SetStats(LevelDifficulty);

        BaseGameMode.Instance.spawnInfo.TotalEnemies--;

        previousPos = spawnPos;

        return enemGO;
    }
}