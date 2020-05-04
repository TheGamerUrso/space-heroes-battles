using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso;
using TheGamerUrso.Utils;
using UnityEngine;
namespace TheGamerUrso
{
    namespace PoolSystem
    {
        [Serializable]
        public class PoolElement
        {
            public string name = "Test";
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
            Planet,
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
            BossProjectile1
        }

        public class PoolManager : Singleton<PoolManager>
        {
            public List<PoolElement> PoolElements;
            public Dictionary<PoolGameObjectType, PoolElement> ListOfPoolElements = new Dictionary<PoolGameObjectType, PoolElement>();
            private List<GameObject> TempNumberOfGameObject;
            private GameObject tempGameObjectPrefab;
            private GameObject holder
                ;
            private void OnValidate()
            {
                //List<PoolGameObjectType> poolGameObjectTypes = Enum.GetValues(typeof(PoolGameObjectType)).Cast<PoolGameObjectType>().ToList();
                //foreach (var item in poolGameObjectTypes)
                //{
                //    foreach (var pElement in PoolElements.ToList())
                //    {
                //        if (pElement.poolGameObjectType != item)
                //        {
                //            PoolElement poolElement = new PoolElement();
                //            poolElement.poolGameObjectType = item;
                //            poolElement.name = item.ToString();
                //            PoolElements.Add(poolElement);
                //        }
                //    }
                //}
            }    

            private void Start()
            {
                if (PoolElements.Count > 0)
                {
                    StartCoroutine(CreatePool());
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

                if (poolGameObjectType == PoolGameObjectType.BulletExplosion || poolGameObjectType == PoolGameObjectType.ShipExplosion)
                {
                    Utilities.SetGameObjectParent(GO.transform, "Effects");
                }

                if (poolGameObjectType == PoolGameObjectType.Planet)
                {
                    Utilities.SetGameObjectParent(GO.transform, "Props");
                }

                if (poolGameObjectType == PoolGameObjectType.FloatingText)
                {
                    Utilities.SetGameObjectParent(GO.transform, "UI");
                }

                if (GO.GetComponent<Items>())
                {
                    Utilities.SetGameObjectParent(GO.transform, "Items");
                }

                if (GO.GetComponent<Ship>())
                {
                    Utilities.SetGameObjectParent(GO.transform, "Ships");
                }

                if (GO.GetComponent<Projectile>())
                {
                    Utilities.SetGameObjectParent(GO.transform, "Projectiles");
                }

                if (GO.GetComponent<Asteroids>())
                {
                    Utilities.SetGameObjectParent(GO.transform, "Asteroids");
                }

                if (holder == null)
                {
                    holder = new GameObject(tempGameObjectPrefab.name);
                    holder.transform.position = Vector3.zero;
                    holder.transform.rotation = Quaternion.identity;
                    holder.transform.parent = this.transform;
                }

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
                        tempGameObjectPrefab.SetActive(true);
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
    }
}