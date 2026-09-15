using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class PoolElement
{
    public string name = "Test";
    public int ID;
    public PoolGameObjectType poolGameObjectType;
    public List<GameObject> PoolElementGameObjects;
    public GameObject PoolElementPrefab;
    public int poolIndex;

    public int GetPoolElementIndex { get { return poolIndex; } }
    public List<GameObject> GetThePoolElementGameObjects { get { return PoolElementGameObjects; } }
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
    Enemy8
}

public class PoolManager : MonoSingleton<PoolManager>
{
    public List<PoolElement> PoolElements;
    public Dictionary<PoolGameObjectType, PoolElement> ListOfPoolElements = new Dictionary<PoolGameObjectType, PoolElement>();
    private List<GameObject> TempNumberOfGameObject;
    private GameObject tempGameObjectPrefab;
    private GameObject holder
        ;
    protected override void Setup()
    {
        base.Setup();
        if (PoolElements.Count > 0)
        {
            StartCoroutine(CreatePool());
        }
    }

    public void Recheck()
    {
        foreach (PoolElement item in PoolElements)
        {
            if (ListOfPoolElements.ContainsKey(item.poolGameObjectType))
            {
                return;
            }
            ListOfPoolElements.Add(item.poolGameObjectType, item);
            CreatePoolByType(item.poolGameObjectType);
        }
    }

    IEnumerator CreatePool()
    {
        foreach (PoolElement item in PoolElements)
        {
            ListOfPoolElements.Add(item.poolGameObjectType, item);
        }

        foreach (PoolGameObjectType type in Enum.GetValues(typeof(PoolGameObjectType)))
        {
            CreatePoolByType(type);
        }

        yield return null;
    }


    public void CreatePool(PoolGameObjectType poolGameObjectType)
    {
        List<GameObject> TempNumberOfGameObject = GetPoolElemet(poolGameObjectType).PoolElementGameObjects;
        GameObject tempGameObjectPrefab = GetPoolElemet(poolGameObjectType).PoolElementPrefab;

        int poolIndex = GetPoolElemet(poolGameObjectType).poolIndex;

        for (int i = 0; i < poolIndex; i++)
        {
            AddGameObjectToPool(poolGameObjectType);
        }
    }

    public void AddGameObjectToPool(PoolGameObjectType poolGameObjectType)
    {
        List<GameObject> TempNumberOfGameObject = GetPoolElemet(poolGameObjectType).PoolElementGameObjects;
        GameObject tempGameObjectPrefab = GetPoolElemet(poolGameObjectType).PoolElementPrefab;

        GameObject GO = Instantiate(tempGameObjectPrefab) as GameObject;

        TempNumberOfGameObject.Add(GO);

        if (holder == null)
        {
            holder = new GameObject(tempGameObjectPrefab.name);
            holder.transform.position = Vector3.zero;
            holder.transform.rotation = Quaternion.identity;
            holder.transform.parent = this.transform;
        }

        HelperUtils.SetGameObjectParent(GO.transform, "DynamicObjects");

        GO.SetActive(false);
    }

    public void CreatePoolByType(PoolGameObjectType poolGameObjectType)
    {
        List<GameObject> TempNumberOfGameObject = GetPoolElemet(poolGameObjectType).GetThePoolElementGameObjects;
        GameObject tempGameObjectPrefab = GetPoolElemet(poolGameObjectType).PoolElementPrefab;

        int poolIndex = GetPoolElemet(poolGameObjectType).GetPoolElementIndex;

        for (int i = 0; i < poolIndex; i++)
        {
            AddGameObjectToPool(poolGameObjectType);
        }
    }

    public List<GameObject> GetPoolByType(PoolGameObjectType poolGameObjectType)
    {
        return GetPoolElemet(poolGameObjectType).PoolElementGameObjects;
    }

    public GameObject GetObjectFromPool(PoolGameObjectType poolGameObjectType)
    {
        TempNumberOfGameObject = GetPoolElemet(poolGameObjectType).PoolElementGameObjects;
        tempGameObjectPrefab = null;

        for (int x = 0; x < TempNumberOfGameObject.Count; x++)
        {
            tempGameObjectPrefab = TempNumberOfGameObject[x];
            if (tempGameObjectPrefab.activeInHierarchy == false)
            {
                return tempGameObjectPrefab;
            }
            else
            {
                if (x == TempNumberOfGameObject.Count - 1)
                {
                    AddGameObjectToPool(poolGameObjectType);
                }
            }
        }
        return null;
    }

    public PoolElement GetPoolElemet(PoolGameObjectType poolGameObjectType)
    {
        PoolElement item;
        if (ListOfPoolElements.TryGetValue(poolGameObjectType, out item))
        {
            return item;
        }
        return null;
    }

}
