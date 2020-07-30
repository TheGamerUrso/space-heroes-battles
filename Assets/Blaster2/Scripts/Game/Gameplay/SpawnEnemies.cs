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

    public static GameObject SpawnBoss(GameObject BossPrefab, int LevelDifficulty = 1)
    {
        AudioManager.Instance.PlayMusicById("Boss", true);
        GameObject currentBoss = GameObject.Instantiate(BossPrefab);
        BaseEnemy enemy = currentBoss.GetComponentInChildren<BaseEnemy>();

        enemy.SetStats(LevelDifficulty);

        BaseGameMode.Instance.TotalEnemies++;
        return currentBoss;
    }

    public static GameObject SpawnEnemyElement(EnemyElement enemyElement, int LevelDifficulty = 1)
    {
        enemyElement.currentNumberInScene++;

        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(Constants.m_XMin, Constants.m_XMax), 0, Constants.m_ZMax);

        Vector3 worldToScreen = Camera.main.WorldToScreenPoint(spawnPos);

        if (enemyElement.gameObjectType == PoolGameObjectType.Enemy2 ||
            enemyElement.gameObjectType == PoolGameObjectType.Enemy6 ||
               enemyElement.gameObjectType == PoolGameObjectType.Enemy7)
        {
            if (previousPos != Vector3.zero)
            {
                Vector3 diff = spawnPos - previousPos;
                if (diff.magnitude <= 10)
                {
                    if (spawnPos.x > previousPos.x)
                    {
                        spawnPos.x += 20;
                    }
                    else if (spawnPos.x <= previousPos.x)
                    {
                        spawnPos.x -= 20;
                    }

                    if (spawnPos.y > previousPos.y)
                    {
                        spawnPos.x += 20;
                    }
                    else if (spawnPos.y <= previousPos.y)
                    {
                        spawnPos.x -= 20;
                    }

                    if (worldToScreen.x + 20 >= Screen.width)
                    {
                        spawnPos.x -= UnityEngine.Random.Range(40, 60);
                    }

                    if (worldToScreen.x - 20 <= 0)
                    {
                        spawnPos.x += UnityEngine.Random.Range(40, 60);
                    }

                    Debug.Log(spawnPos + " : " + worldToScreen + " : " + diff);
                }
            }

        }

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

        BaseGameMode.Instance.TotalEnemies--;

        previousPos = spawnPos;

        return enemGO;
    }
}