using System;
using UnityEngine;

public class Turret : Ship
{
    [SerializeField] private BaseWeapon baseWeapon;

    public void ExitLevel() => Destroy(gameObject);

    public override void SetStats(int level,
        float baseHealth,
        float baseSpeed, 
        float baseDamage, 
        float baseFireRate, 
        float baseSuperDamage = 0, 
        float baseSpecialCountdown = 0)
    {
        stats.Level = Mathf.Clamp(level, 1, 10);

        stats.Health = level * baseHealth;
        stats.Speed = baseSpeed;
        stats.Damage = (level * baseDamage);
        stats.FireRate = baseFireRate;

        healthComponent.Setup(stats.Health, false);
        weaponController.Setup(this, stats.Damage, stats.FireRate);
        movementController.SetSpeed(stats.Speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.ENEMYTAG))
        {
            healthComponent.TakeDamage(1);
        }
        if (other.tag.Equals(Constants.ENEMYPROJECTILETAG))
        {
            healthComponent.TakeDamage(1);
        }
    }

}