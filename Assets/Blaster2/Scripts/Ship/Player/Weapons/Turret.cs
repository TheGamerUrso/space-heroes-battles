using System;
using UnityEngine;

public class Turret : Ship, IDamagable
{
    private PlayerShipData playerShipData;
    private PlayerData playerData;
    [SerializeField] private BaseWeapon baseWeapon;

    public override void ExitLevel() => Destroy(gameObject);

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

    public override void TakeDamage(float dmg)
    {
        CurrentHealth -= dmg;
        if (CurrentHealth < 0)
        {
            Death();
        }
    }

    public override void Heal(float ammount) { }

    public override void Death() => Destroy(gameObject);

    public override float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth);
    }

    public override void SwitchWeapon(int Id,bool Solo = false)
    {
       // No Other Weapons
    }

    public override void EnterLevel()
    {
       
    }

}