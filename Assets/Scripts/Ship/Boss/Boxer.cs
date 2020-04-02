using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boxer : BaseBossEnemy
{
    public override void TakeDamage(float damage)
    {
        if (DestroyableParts.Count > 0)
        {
            foreach (IDestroyable item in DestroyableParts)
            {
                if (item.IsDestroyed == false)
                {
                    return;
                }
            }

        }

        base.TakeDamage(damage);
    }


    public override void BossHit()
    {
 

        base.BossHit();
    }
}
