using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyElement> enemyElements;
    public int LevelDifficulty;
    public GameObject SpawnEnemyElement(int availableEnemies)
    {
        GameController.Instance.NumberOfEnemies++;

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

        BaseEnemyMovement enemyMovement = enemGO.GetComponent<BaseEnemyMovement>();
        enemGO.transform.position = spawnPos;
        enemGO.transform.rotation = Quaternion.LookRotation(Vector3.back);

        enemGO.SetActive(true);

        FollowPathEnemyMovement followPathEnemyMovement = enemyMovement.GetComponent<FollowPathEnemyMovement>();
        if (followPathEnemyMovement)
        {
            enemy.enemyElement = enemyElement;

            int pathIndex = followPathEnemyMovement.GeneratePath();

            if (enemyElement.gameObjectType == PoolGameObjectType.Enemy4)
            {
                followPathEnemyMovement.PingPong = false;
                if (pathIndex == 0)
                {
                    followPathEnemyMovement.PingPong = true;
                }
            }
        }
        return enemGO;
    }

    public GameObject SpawnEnemyElement()
    {
        GameController.Instance.NumberOfEnemies++;

        var spawnPos = transform.position;
        var randEnemyIndex = Random.Range(0, enemyElements.Count);
        var enemyElement = enemyElements[randEnemyIndex];
        var enemGO = PoolManager.Instance.GetObjectFromPool(enemyElement.gameObjectType);
        var enemy = enemGO.GetComponent<Enemy>();

        enemy.SetStats(LevelDifficulty);
        enemy.Id = enemyElement.Name;

        enemGO.transform.position = spawnPos;
        enemGO.transform.rotation = Quaternion.LookRotation(Vector3.back);
        enemGO.SetActive(true);

        FollowPathEnemyMovement enemyMovement = enemGO.GetComponent<FollowPathEnemyMovement>();
        if (enemyMovement)
        {
           enemyMovement.Initialize(enemyElement);
        }
        return enemGO;
    }
}
