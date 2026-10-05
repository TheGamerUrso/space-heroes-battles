using System;
using UnityEngine;

public class BossWeaponController : WeaponController
{
    public event Action<BossEnemy, int> OnBossAttacked;
    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].AutoAttack = true;
        }
    }
    protected void Attack()
    {
        if (delayAttak > 0)
        {
            delayAttak -= Time.deltaTime;
        }
    }

}
