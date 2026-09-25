using System;
using UnityEngine;

public class BossWeaponController : WeaponController
{
    public event Action<BossEnemy, int> OnBossAttacked;

    protected override void Attack()
    {
        if (delayAttak > 0)
        {
            delayAttak -= Time.deltaTime;
        }
    }

}
