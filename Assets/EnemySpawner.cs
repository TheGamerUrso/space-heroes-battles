using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyElement> enemyElements;
    public int LevelDifficulty;
    public GameObject SpawnEnemyElement(int availableEnemies)
    {
        Game.NumberOfEnemies++;

        if (availableEnemies > enemyElements.Count)
        {
            availableEnemies = enemyElements.Count;
        }

        var spawnPos = transform.position;
        var randEnemyIndex = Random.Range(0, availableEnemies);
        var enemyElement = enemyElements[randEnemyIndex];
        var enemGO = PoolManager.Instance.GetObjectFromPool(enemyElement.gameObjectType);
        var enemy = enemGO.GetComponent<Enemy>();

        enemy.SetStats(LevelDifficulty);
        enemy.Id = enemyElement.Name;
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
        }
        return enemGO;
    }

    public GameObject SpawnEnemyElement()
    {
        Game.NumberOfEnemies++;

        var spawnPos = transform.position;
        var randEnemyIndex = Random.Range(0, enemyElements.Count);
        var enemyElement = enemyElements[randEnemyIndex];
        var enemGO = PoolManager.Instance.GetObjectFromPool(enemyElement.gameObjectType);
        var enemy = enemGO.GetComponent<Enemy>();

        enemy.SetStats(LevelDifficulty);
        enemy.Id = enemyElement.Name;
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
        }
        return enemGO;
    }
}
