using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWithWeapons : Enemy
{
    public override void OnEnable()
    {
        base.OnEnable();

        DisableAllWeapons();

        if (AutoEnableWeapon)
        {
            EnableAllWeapon();
        }
    }

    public override void Start()
    {
        base.Start();
        StartCoroutine(ActivateWeapons());
    }

    IEnumerator ActivateWeapons()
    {
        yield return new WaitForSeconds(2);

        EnableWeaponById(0);

        if (HealthBar != null)
        {
            HealthBar.Show();
        }

        EnableColliders(true);
    }

}
