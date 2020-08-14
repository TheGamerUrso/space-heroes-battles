using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossBattleSystem : BaseBossBattle
{
    public override void Phases()
    {
        HealthPresentage = bossEnemy.GetHealthPresentage();
        if (phase == 0 && HealthPresentage <= 80f)
        {
            phase = 1;
            bossEnemy.SetFireRate();
        }
        else if (phase == 1 && HealthPresentage <= 60f)
        {
            phase = 2;
            bossEnemy.SetFireRate();
            bossEnemy.EnableWeaponById(1);
        }
        else if (phase == 2 && HealthPresentage <= 30f)
        {
            phase = 3;
            bossEnemy.SetFireRate();
        }
    }


    public override void BossHit(int hitIndex, int numberOfHits)
    {
        if (GuiManager.Instance.IsTrasnmiting() || DelayAttack > 0)
        {
            return;
        }

        PlayerData playerData = PersistantData.GetPlayerData();

        if (playerData != null)
        {
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.05f);
        }

        hitIndex++;

        if (hitIndex > numberOfHits)
        {
            hitIndex = 0;
            bossAI.ChangeWaypoint(hitIndex);
        }
    }
}
