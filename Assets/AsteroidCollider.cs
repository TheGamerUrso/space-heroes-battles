using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidCollider : MonoBehaviour, IDamagable
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

    private Rigidbody rigid;
    private float force = 2500;



    private void Awake()
    {
        rigid = GetComponentInParent<Rigidbody>();
    }
    private void Start()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();
        maxHealth = maxHealth * playerShipData.level;
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
            var destroyable = other.gameObject.GetComponent<IDamagable>();
            destroyable.TakeDamage(destroyable.MaxHealth / 2);
            TakeDamage(destroyable.MaxHealth);
        }
    }
}
