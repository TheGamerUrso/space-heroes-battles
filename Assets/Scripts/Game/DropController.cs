using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DropProbabilities
{
    public string Name;
    public PoolGameObjectType DropItemsType;
}

public class DropController : MonoBehaviour
{
    private static DropController instance;

    public static DropController Instance
    {
        get
        {
            try
            {
                return instance;
            }
            catch (NullReferenceException e)
            {
                Debug.LogWarning(e.Message);
            }
            return null;
        }
    }

    private float timerSincePowerUpDroped;

    private float frequentToDroPowerUp = .1f;

    public List<DropProbabilities> ListOfDropItems = new List<DropProbabilities>();

    public bool DropShield;
    public bool DropHealth;

    public float shieldDropCooldown = 4;
    public float healthDropCooldown = 3;
    public float powerDropCooldown = 1;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(powerDropCooldown > 0)
        {
            powerDropCooldown -= Time.deltaTime;
        }

        if (shieldDropCooldown > 0)
        {
            shieldDropCooldown -= Time.deltaTime;
        }

        if (healthDropCooldown > 0)
        {
            healthDropCooldown -= Time.deltaTime;
        }
    }

    public static void PickRandomDropItem(Transform transform)
    {
        DropController.instance.PickRandomEnemyToSpawn(transform);
    }

    public void PickRandomEnemyToSpawn(Transform transform)
    {

        if (ListOfDropItems.Count > 0)
        {
            if (PlayerManager.GetPlayer() == null)
            {
                return;
            }

            Player p = PlayerManager.GetPlayer();


            bool hasShield = p.HasShieldModule();
            bool fullHealth = p.GetHealthPresentage() == 1;
            bool dropExtra = false;

            PoolGameObjectType itemTypeToSpawn = ListOfDropItems[0].DropItemsType;

            do
            {
                itemTypeToSpawn = ListOfDropItems[UnityEngine.Random.Range(0, ListOfDropItems.Count)].DropItemsType;

                if (itemTypeToSpawn == PoolGameObjectType.ItemShield)
                {
                    if (!hasShield && shieldDropCooldown < 0)
                    {
                        itemTypeToSpawn = ListOfDropItems[2].DropItemsType;
                        shieldDropCooldown = 2;
                        break;
                    }
                    else
                    {
                        itemTypeToSpawn = ListOfDropItems[0].DropItemsType;
                    }
                }

                if (itemTypeToSpawn == PoolGameObjectType.ItemHealth)
                {
                    if (!fullHealth && healthDropCooldown < 0)
                    {
                        itemTypeToSpawn = ListOfDropItems[3].DropItemsType;
                        healthDropCooldown = 2;
                        break;
                    }
                    else
                    {
                        itemTypeToSpawn = ListOfDropItems[0].DropItemsType;
                    }
                }

                if (itemTypeToSpawn == PoolGameObjectType.ItemPowerUp)
                {
                    if (powerDropCooldown < 0)
                    {
                        itemTypeToSpawn = ListOfDropItems[1].DropItemsType;
                        powerDropCooldown = 2;
                        break;
                    }
                    else
                    {
                        itemTypeToSpawn = ListOfDropItems[0].DropItemsType;
                    }
                }
            } while (itemTypeToSpawn != PoolGameObjectType.ItemCoin);


            if (itemTypeToSpawn != PoolGameObjectType.ItemCoin)
            {
                dropExtra = true;
            }

            if (dropExtra)
            {
                GameObject extraDrop = PoolManager.Instance.GetObjectFromPool(itemTypeToSpawn);
                extraDrop.transform.position = transform.position;
                extraDrop.transform.rotation = Quaternion.identity;

                GameObject drop = PoolManager.Instance.GetObjectFromPool(ListOfDropItems[0].DropItemsType);
                drop.transform.position = transform.position;
                drop.transform.rotation = Quaternion.identity;
            }
            else if (dropExtra == false)
            {
                GameObject extraDrop = PoolManager.Instance.GetObjectFromPool(ListOfDropItems[0].DropItemsType);
                extraDrop.transform.position = transform.position;
                extraDrop.transform.rotation = Quaternion.identity;
            }

            SpawnEnemies.CoinDropInTotal++;

            return;
        }

    }
}