using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boxer : BaseBossEnemy
{
    public override void Phases()
    {
        if (Weapons[0].activeSelf == false)
        {
            Weapons[0].SetActive(true);
        }
    }
    public override void TakeDamage(float damage)
    {

        foreach (IDestroyable item in DestroyableParts)
        {
            if (item.IsAlive == false)
            {
                return;
            }
        }


        base.TakeDamage(damage);
    }


    public override void BossTakeDamage()
    {
        if (GuiManager.IsTrasnmiting() || delayAttak > 0)
        {
            return;
        }

      

        PlayerWeaponSystem playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
        playerWeaponSystem.IncreasePowerUp(.05f);

        if (Random.Range(0, 100) >= 50)
        {
            if (GetHealthPresentage() % 40 == 0)
            {
                GetComponent<BossAI>().ChangeWaypoint(hitIndex);
            }
        }


        base.BossTakeDamage();
    }
}
