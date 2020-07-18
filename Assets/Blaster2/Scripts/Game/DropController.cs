using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DropProbabilities
{
    public string Name;
    public int chance;
    public PoolGameObjectType DropItemsType;
}

public class DropController : MonoSingleton<DropController>
{
    //private float timerSincePowerUpDroped;

   // private float frequentToDroPowerUp = .1f;

    public List<DropProbabilities> ListOfDropItems = new List<DropProbabilities>();

    public bool DropShield;
    public bool DropHealth;

    public float shieldDropCooldown = 4;
    public float healthDropCooldown = 3;
    public float powerDropCooldown = 1;

    private void Update()
    {
        if (powerDropCooldown > 0)
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
        Instance.PickRandomEnemyToSpawn(transform);
    }

    public void PickRandomEnemyToSpawn(Transform transform)
    {

        if (ListOfDropItems.Count > 0)
        {
            if (PlayerManager.GetPlayer() == null)
            {
                return;
            }

            PlayerShip p = PlayerManager.GetPlayer();


            bool hasShield = p.HasShieldModule();
            bool fullHealth = p.HealthPresentage == 100;
            bool dropExtra = false;

            PoolGameObjectType itemTypeToSpawn = ListOfDropItems[0].DropItemsType;

            do
            {

                var range = 0;

                for (int i = 0; i < ListOfDropItems.Count; i++)
                {
                    if (ListOfDropItems[i].chance > 0f)
                    {
                        range += ListOfDropItems[i].chance;
                    }
                }

                var rand = UnityEngine.Random.Range(0, range);
                var top = 0;

                for (int i = 0; i < ListOfDropItems.Count; i++)
                {
                    top += ListOfDropItems[i].chance;
                    if (rand < top)
                    {
                        itemTypeToSpawn = ListOfDropItems[i].DropItemsType;
                        break;
                    }
                }            

                if (itemTypeToSpawn == PoolGameObjectType.ItemShield)
                {
                    if (!hasShield && shieldDropCooldown < 0)
                    {
                        itemTypeToSpawn = ListOfDropItems[2].DropItemsType;
                        shieldDropCooldown = UnityEngine.Random.Range(2, 8);
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
                        healthDropCooldown = UnityEngine.Random.Range(2, 8);
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
                        powerDropCooldown = UnityEngine.Random.Range(2, 4);
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
                GameSession.CoinDropInTotal++;
            }

            //TODO Coins Drop In Total ++
             return;
        }

    }
}