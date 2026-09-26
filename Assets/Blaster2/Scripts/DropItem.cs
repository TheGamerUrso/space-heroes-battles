using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

[Serializable]
public class DropProbabilities
{
    public string Name;
    public int chance;
    public PoolGameObjectType DropItemsType;
}

public class DropItem : MonoBehaviour
{
    [SerializeField] private List<DropProbabilities> ListOfDropItems = new List<DropProbabilities>();

    [SerializeField] private bool DropShield;
    [SerializeField] private bool DropHealth;

    [SerializeField] private float shieldDropCooldown = 4;
    [SerializeField] private float healthDropCooldown = 3;
    [SerializeField] private float powerDropCooldown = 1;
    [SerializeField] private GameController gameController;

    private PoolGameObjectType itemTypeToSpawn;
    private PlayerShip playerShip;
    private IEventService eventService;

    private void Start()
    {
        eventService = GameContext.Get<IEventService>();
        eventService.Subscribe<EnemyDiedEvent>(EnemyDiedEventHanded);
    }
    private void OnDestroy()
    {
        eventService.Unsubscribe<EnemyDiedEvent>(EnemyDiedEventHanded);
    }

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
    private void PickRandomEnemyToSpawn(Transform transform)
    {
        if (playerShip == null)
            playerShip = gameController.GetPlayer();

        if (ListOfDropItems.Count > 0)
        {
            bool hasShield = playerShip.GetComponent<HealthComponent>().HasShield;
            bool fullHealth = playerShip.GetComponent<HealthComponent>().GetHealthPresentage() == 1;
            bool dropExtra = false;

            itemTypeToSpawn = ListOfDropItems[0].DropItemsType;

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


                extraDrop.SetActive(true);
                drop.SetActive(true);
            }
            else if (dropExtra == false)
            {
                GameObject extraDrop = PoolManager.Instance.GetObjectFromPool(ListOfDropItems[0].DropItemsType);
                extraDrop.transform.position = transform.position;
                extraDrop.transform.rotation = Quaternion.identity;

                extraDrop.SetActive(true);
            }
            return;
        }

    }
    private void EnemyDiedEventHanded(EnemyDiedEvent payload)
    {
        PickRandomEnemyToSpawn(payload.enemy.transform);
    }
}
