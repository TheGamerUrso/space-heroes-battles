using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct EnemyElement
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
    public static GameObject SpawnBoss(GameObject BossPrefab, int LevelDifficulty = 1)
    {
        AudioManager.Instance.PlayMusicById("Boss", true);
        GameObject currentBoss = GameObject.Instantiate(BossPrefab);
        BaseEnemy enemy = currentBoss.GetComponent<BaseEnemy>();

        enemy.SetEnemyStats(LevelDifficulty);

        BaseGameMode.Instance.TotalEnemies++;
        return currentBoss; 
    }

    public static GameObject SpawnEnemyElement(EnemyElement enemyElement, int LevelDifficulty = 1)
    {
        enemyElement.currentNumberInScene++;
        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(Constants.m_XMin, Constants.m_XMax), 0, Constants.m_ZMax);
        GameObject enemGO = PoolManager.Instance.GetObjectFromPool(enemyElement.gameObjectType);
        BaseEnemy enemy = enemGO.GetComponent<BaseEnemy>();
        FollowPathAI followPathAI = enemGO.GetComponent<FollowPathAI>();

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

        enemy.SetEnemyStats(LevelDifficulty);

        BaseGameMode.Instance.TotalEnemies--;

        return enemGO;
    }
}