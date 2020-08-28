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
    public bool enemy1Spawned;
    private int previousIndex = 0;
    private int previousPath = 0;
    EnemyElement prevEnemyElement;


    public SpawnEnemies(List<SpawnPoint> spawnPoints)
    {
        this.spawnPoints = spawnPoints;
    }

    public static BossEnemy SpawnBoss(GameObject BossPrefab, int LevelDifficulty = 1)
    {
        AudioManager.Instance.PlayMusicById("Boss", true);
        GameObject currentBoss = GameObject.Instantiate(BossPrefab);
        currentBoss.name = BossPrefab.name;

        BossEnemy enemy = currentBoss.GetComponentInChildren<BossEnemy>();
        enemy.Id = currentBoss.name;
        enemy.SetStats(LevelDifficulty);

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
        Vector3 spawnPos = tempList[spawnIndex].spawnPoint.transform.position;



        if (enemyElement.gameObjectType == PoolGameObjectType.Enemy1)
        {
            if (enemy1Spawned)
            {
                previousIndex = spawnIndex;
            }
        }
        else
        {
            enemy1Spawned = false;
            spawnPoints[previousIndex].used = false;
            spawnPoints[spawnIndex].used = true;
        }

        GameController.Instance.StartCoroutine(EnableSpawnPointAgain(spawnPoints[spawnIndex]));

        GameObject enemGO = PoolManager.Instance.GetObjectFromPool(enemyElement.gameObjectType);

        Enemy enemy = enemGO.GetComponent<Enemy>();
        enemy.SetStats(LevelDifficulty);

        enemy.Id = enemyElement.Name + "_" + enemyElement.currentNumberInScene;
        EnemyMove followPathAI = enemGO.GetComponent<EnemyMove>();

        enemGO.transform.position = spawnPos;
        
        enemGO.transform.rotation = Quaternion.LookRotation(Vector3.back);
       
        enemGO.SetActive(true);

        if (followPathAI.moveType == EnemyMove.EnemyMoveType.FollowPath)
        {
            enemy.enemyElement = enemyElement;

            int pathIndex = followPathAI.GeneratePath();

            if (enemyElement.gameObjectType == PoolGameObjectType.Enemy4)
            {
                followPathAI.PingPong = false;
                if (pathIndex == 0)
                {
                    followPathAI.PingPong = true;
                }
            }

            if (enemyElement.gameObjectType == PoolGameObjectType.Enemy1)
            {
                if (enemy1Spawned)
                {
                    enemGO.transform.position = previousPos;
                    followPathAI.GeneratePathByIndex(previousPath);
                }
                else
                {
                    enemy1Spawned = true;
                    previousPath = pathIndex;
                    enemGO.transform.position = spawnPos;
                }
            }

            previousPos = spawnPos;
            prevEnemyElement = enemyElement;
        }


        return enemGO;
    }

}