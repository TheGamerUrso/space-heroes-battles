using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ShipStatsSystem
{
    private float MaxHealth;
    public float CurrentHealth;

    public float Damage;
    [Range(.1f, 10)]
    public float FireRate;
    public float Speed;
    public float SuperDamage;
    public float SuperChargeTime;

    public float MagnetPower;
    public float MagnetDistance;


    [Header("Base Attributes")]
    public float baseDamage;
    public float baseHealth;
    [Range(.1f,10)]
    public float baseFireRate;
    public float baseSpecialCountdown;

    public ShipStatsSystem()
    {
        baseDamage = 12.5f;
        baseHealth = 25f;
        baseFireRate = .4f;
        baseSpecialCountdown = 1f;
    }

    public void ReplaceBaseStats(ShipStatsSystem shipStatsSystem)
    {
        this.baseDamage = shipStatsSystem.baseDamage;
        this.baseHealth = shipStatsSystem.baseHealth;
        this.baseFireRate = shipStatsSystem.baseHealth;
        this.baseSpecialCountdown = shipStatsSystem.baseSpecialCountdown;
    }

    public void SetStats(LevelSystem levelSystem)
    {
        var multiplier = levelSystem.GetLevel() / 10;
        MaxHealth = levelSystem.GetLevel() * baseHealth;
        CurrentHealth = MaxHealth;
        Damage = levelSystem.GetLevel() * baseDamage;
        FireRate = baseFireRate;
        SuperDamage = levelSystem.GetLevel() * baseDamage;
    }

    public float GetHealthPressentage()
    {
        return (CurrentHealth/MaxHealth) * 100;
    }

    public bool CheckHealthPressentage(float pressent)
    {
        if (CurrentHealth <= (pressent * MaxHealth))
        {
            return true;
        }
        return false;
    }

    public float GetSuperTime()
    {
        return SuperChargeTime;
    }
    public float GetMaxHealth()
    {
        return MaxHealth;
    }
    public void SetMaxHealth(float ammount)
    {
        MaxHealth = ammount;
    }

    public float GetCurrentHealth()
    {
        return CurrentHealth;
    }

    public float GetSpeed()
    {
        return Speed;
    }
}
