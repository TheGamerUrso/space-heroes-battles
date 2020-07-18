using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boxer : BaseBossEnemy
{
    public override void TakeDamage(float damage)
    {
        int destroyed = 0;
        for (int i = 0; i < DestroyableParts.Count; i++)
        {
            IDamagable item = DestroyableParts[i];
            if (item.CurrentHealth <= 0)
            {
                destroyed++;
            }
        }
        if (destroyed == 2)
        {
            base.TakeDamage(damage);
        }
    }
}
