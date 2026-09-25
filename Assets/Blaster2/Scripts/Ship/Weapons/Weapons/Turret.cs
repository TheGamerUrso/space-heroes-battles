using System;
using UnityEngine;

public class Turret : Ship
{
    [SerializeField] private BaseWeapon baseWeapon;

    public void ExitLevel() => Destroy(gameObject);


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