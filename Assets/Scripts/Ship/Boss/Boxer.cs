using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boxer : BaseBossEnemy
{
    public override void TakeDamage(float damage)
    {
        foreach (IDestroyable item in DestroyableParts)
        {
            if (item.IsDestroyed == false)
            {
                return;
            }
        }


        base.TakeDamage(damage);
    }


    public override void BossHit()
    {
        if (GuiManager.IsTrasnmiting() || delayAttak > 0)
        {
            return;
        }

        PlayerWeaponSystem playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
        if(playerWeaponSystem)
        playerWeaponSystem.IncreasePowerUp(.05f);

        if (Random.Range(0, 100) >= 50)
        {
            if (GetHealthPresentage() % 40 == 0)
            {
                GetComponent<BossAI>().ChangeWaypoint(hitIndex);
            }
        }

        base.BossHit();
    }
}
