using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SpawnLocations
{
    public bool IsAvailable;
    public float cooldown;
    public Transform location;
}

public class EnemySpawner : MonoBehaviour
{
    public List<PoolGameObjectType> gameObjectTypeList;
    public int LevelDifficulty;

    public List<SpawnLocations> spawnLocations;

    private void Update()
    {
        foreach (var item in spawnLocations)
        {
            if (item.IsAvailable) continue;
            item.cooldown -= Time.deltaTime;
            if (item.cooldown <= 0)
            {
                item.IsAvailable = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SpawnEnemyElement(availableEnemy);
        }
    }

    public GameObject SpawnEnemyElement(int availableEnemies)
    {
        if (availableEnemies > gameObjectTypeList.Count)
        {
            availableEnemies = gameObjectTypeList.Count;
        }
        var availableSpawnLocations = spawnLocations.Where(x => x.IsAvailable).ToList();

        if (availableSpawnLocations.Count == 0) return null;

        var spawnPos = availableSpawnLocations[UnityEngine.Random.Range(0, availableSpawnLocations.Count)];
        var randEnemyIndex = UnityEngine.Random.Range(0, availableEnemies);
        var gameObjectType = gameObjectTypeList[randEnemyIndex];
        var enemGO = PoolManager.Instance.GetObjectFromPool(gameObjectType);
        var enemy = enemGO.GetComponent<Enemy>();
        enemy.GameObjectType = gameObjectType;

        enemy.SetStats(LevelDifficulty);
 
        var enemyMovement = enemGO.GetComponent<BaseEnemyMovement>();
        enemyMovement.Setup(spawnPos.location.position, Quaternion.LookRotation(Vector3.back));

        enemGO.SetActive(true);
        return enemGO;
    }

    public GameObject SpawnSpecificEnemy(PoolGameObjectType enemyType)
    {
        var availableSpawnLocations = spawnLocations.Where(x => x.IsAvailable).ToList();
        if (availableSpawnLocations.Count == 0) return null;

        var spawnPos = availableSpawnLocations[UnityEngine.Random.Range(0, availableSpawnLocations.Count)];
        var enemGO = PoolManager.Instance.GetObjectFromPool(enemyType);
        var enemy = enemGO.GetComponent<Enemy>();
        enemy.GameObjectType = enemyType;

        enemy.SetStats(LevelDifficulty);

        var enemyMovement = enemGO.GetComponent<BaseEnemyMovement>();
        enemyMovement.Setup(spawnPos.location.position, Quaternion.LookRotation(Vector3.back));

        enemGO.SetActive(true);
        return enemGO;
    }

    public int availableEnemy = 0;
    [ContextMenu("Debug_Spawn")]
    public void Debug_SpawnEnemyElement()
    {
        SpawnEnemyElement(availableEnemy); 
    }
}
