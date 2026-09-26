using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<PoolGameObjectType> gameObjectTypeList;
    public int LevelDifficulty;
    public GameObject SpawnEnemyElement(int availableEnemies)
    {
        if (availableEnemies > gameObjectTypeList.Count)
        {
            availableEnemies = gameObjectTypeList.Count;
        }

        var spawnPos = transform.position;
        var randEnemyIndex = Random.Range(0, availableEnemies);
        var gameObjectType = gameObjectTypeList[randEnemyIndex];
        var enemGO = PoolManager.Instance.GetObjectFromPool(gameObjectType);
        var enemy = enemGO.GetComponent<Enemy>();
        enemy.GameObjectType = gameObjectType;

        var enemyData = enemy.EnemyData;
        enemy.SetStats(LevelDifficulty, enemyData.baseHealth, enemyData.baseSpeed, enemyData.baseDamage, enemyData.baseFireRate);
 
        var enemyMovement = enemGO.GetComponent<BaseEnemyMovement>();
        enemyMovement.Setup(spawnPos, Quaternion.LookRotation(Vector3.back));

        enemGO.SetActive(true);
        return enemGO;
    }

    public GameObject SpawnEnemyElement()
    {
        var spawnPos = transform.position;
        var randEnemyIndex = Random.Range(0, gameObjectTypeList.Count);
        var gameObjectType = gameObjectTypeList[randEnemyIndex];
        var enemGO = PoolManager.Instance.GetObjectFromPool(gameObjectType);
        var enemy = enemGO.GetComponent<Enemy>();
        enemy.GameObjectType = gameObjectType;

        var enemyData = enemy.EnemyData;
        enemy.SetStats(LevelDifficulty, enemyData.baseHealth, enemyData.baseSpeed, enemyData.baseDamage, enemyData.baseFireRate);


        BaseEnemyMovement enemyMovement = enemGO.GetComponent<BaseEnemyMovement>();
        enemyMovement.Setup(spawnPos, Quaternion.LookRotation(Vector3.back));

        enemGO.SetActive(true);
        return enemGO;
    }
}
