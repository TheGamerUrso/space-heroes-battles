using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;

public class AsteroidCollider : MonoBehaviour, IDamagable, ITargetable
{
    public event Action<float, float> OnHealthChanged;
    private bool Destroyed = false;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    public GameObject target
    {
        get
        {
            return gameObject;
        }
    }

    public bool Targetable
    {
        get
        {
            return !Destroyed;
        }
    }


    private Rigidbody rigid;
    private float force = 2500;

    private IDataService dataService;

    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();


        rigid = GetComponentInParent<Rigidbody>();

        PlayerData playerData = dataService.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();
        SetMaxHealth(playerShipData.level);

    }
    private void Start()
    {
        
    }

    private void SetMaxHealth(float level)
    {
        maxHealth = maxHealth * level;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        if (Destroyed == true)
        {
            return;
        }
        CurrentHealth -= dmg;
        if (CurrentHealth <= 0)
        {
            Death();
        }
    }
    public void Heal(float ammount) { }

    public void Death()
    {
        Destroyed = true;

        var explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
        explostion.transform.position = transform.position;
        explostion.SetActive(true);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(Constants.PLAYTERTAG))
        {
            var ship = other.gameObject.GetComponent<Ship>();
            ship.TakeDamage(ship.MaxHealth / 2);
            TakeDamage(ship.MaxHealth);
        }
    }
}
