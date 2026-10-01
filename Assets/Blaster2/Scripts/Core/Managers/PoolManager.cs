using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PoolElement
{
    [Tooltip("The type identifier for this pool.")]
    public PoolGameObjectType poolGameObjectType;

    [Tooltip("The prefab to instantiate.")]
    public GameObject poolElementPrefab;

    [Tooltip("Initial number of objects to pool.")]
    public int poolIndex = 10;

    // Hidden from the Inspector because it is managed entirely at runtime
    [HideInInspector]
    public List<GameObject> poolElementGameObjects = new List<GameObject>();
}

[Serializable]
public enum PoolGameObjectType
{
    PlayerProjectile,
    PlayerRocket,
    EnemyProjectile,
    EnemyProjectileBall,
    EnemyProjectileRocket,
    EnemyGrenadeProjectile,
    BulletExplosion,
    ShipExplosion,
    ItemCoin,
    ItemPowerUp,
    ItemShield,
    ItemHealth,
    FloatingText,
    Enemy1,
    Enemy2,
    Enemy3,
    Enemy4,
    Enemy5,
    Enemy6,
    Asteroid1,
    Asteroid2,
    Asteroid3,
    Asteroid4,
    Asteroid5,
    Asteroid6,
    Asteroid7,
    Asteroid8,
    Asteroid9,
    Enemy7,
    Boss3Artilery,
    BossProjectile1,
    Enemy8,
    Enemy9
}

public class PoolManager : MonoSingleton<PoolManager>
{
    [Header("Pool Databases")]
    [SerializeField] private List<PoolDatabase> poolDatabases = new List<PoolDatabase>();

    private Dictionary<PoolGameObjectType, PoolElement> listOfPoolElements = new Dictionary<PoolGameObjectType, PoolElement>();
    private Dictionary<PoolGameObjectType, Transform> poolHolders = new Dictionary<PoolGameObjectType, Transform>();

    protected override void Setup()
    {
        base.Setup();
        if (poolDatabases != null && poolDatabases.Count > 0)
        {
            StartCoroutine(CreatePoolRoutine());
        }
        else
        {
            Debug.LogError("No Pool Databases assigned to PoolManager!");
        }
    }

    private IEnumerator CreatePoolRoutine()
    {
        // Loop through every assigned database file
        foreach (PoolDatabase database in poolDatabases)
        {
            if (database == null || database.poolElements == null) continue;

            foreach (PoolElement item in database.poolElements)
            {
                if (item.poolElementPrefab == null) continue;

                if (!listOfPoolElements.ContainsKey(item.poolGameObjectType))
                {
                    listOfPoolElements.Add(item.poolGameObjectType, item);
                    CreatePoolByType(item.poolGameObjectType);
                }
                else
                {
                    Debug.LogWarning($"Duplicate pool type detected: {item.poolGameObjectType} in database {database.name}");
                }
            }
        }
        yield return null;
    }

    public GameObject AddGameObjectToPool(PoolGameObjectType poolGameObjectType)
    {
        PoolElement poolElement = GetPoolElement(poolGameObjectType);
        if (poolElement == null || poolElement.poolElementPrefab == null) return null;

        GameObject go = Instantiate(poolElement.poolElementPrefab);
        go.SetActive(false);

        if (!poolHolders.TryGetValue(poolGameObjectType, out Transform holderTransform))
        {
            GameObject holderGO = new GameObject($"Pool_{poolGameObjectType}");
            holderGO.transform.SetParent(this.transform);
            holderTransform = holderGO.transform;
            poolHolders.Add(poolGameObjectType, holderTransform);
        }

        go.transform.SetParent(holderTransform);
        poolElement.poolElementGameObjects.Add(go);

        return go;
    }

    public GameObject GetObjectFromPool(PoolGameObjectType poolGameObjectType)
    {
        PoolElement poolElement = GetPoolElement(poolGameObjectType);
        if (poolElement == null) return null;

        for (int x = 0; x < poolElement.poolElementGameObjects.Count; x++)
        {
            GameObject obj = poolElement.poolElementGameObjects[x];
            if (obj != null && !obj.activeInHierarchy)
            {
                return obj;
            }
        }

        return AddGameObjectToPool(poolGameObjectType);
    }

    public PoolElement GetPoolElement(PoolGameObjectType poolGameObjectType)
    {
        if (listOfPoolElements.TryGetValue(poolGameObjectType, out PoolElement item))
        {
            return item;
        }
        return null;
    }

    private void CreatePoolByType(PoolGameObjectType poolGameObjectType)
    {
        PoolElement poolElement = GetPoolElement(poolGameObjectType);
        if (poolElement == null) return;

        for (int i = 0; i < poolElement.poolIndex; i++)
        {
            AddGameObjectToPool(poolGameObjectType);
        }
    }
}