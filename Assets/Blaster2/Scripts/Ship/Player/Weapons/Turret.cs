using System;
using UnityEngine;

public class Turret : Ship, IDamagable
{
    private PlayerShipData playerShipData;
    private PlayerData playerData;

    public PlayerWeapon playerWeapon;
    public bool IsAlive { get; set; }
   public float MaxHealth { get; set; }

    public float CurrentHealth { get; set; }

    public event Action<float, float> OnHealthChanged;

    public override void OnEnable()
    {
        playerWeapon.weaponData.Damage = playerShipData.SuperDamage;
    }

    public override void Start()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
    }

    public void Deactivate()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.ENEMYTAG))
        {
            TakeDamage(1);
        }
        if (other.tag.Equals(Constants.ENEMYPROJECTILETAG))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(float dmg)
    {
        CurrentHealth -= dmg;
        if (CurrentHealth < 0)
        {
            Death();
        }
    }

    public void Heal(float ammount)
    {
     
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}