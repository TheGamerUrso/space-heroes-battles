using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossBattle : MonoBehaviour
{
    public BaseBossEnemy bossEnemy;
    public BaseBossEnemyAI bossAI;
    [SerializeField] protected float HealthPresentage;
    [SerializeField] protected int phase;
    private void OnDestroy()
    {
        bossEnemy.OnBossAttack -= Phases;
    }
    private void Start()
    {
        bossAI = GetComponent<BaseBossEnemyAI>();
        bossEnemy = GetComponent<BaseBossEnemy>();
        bossEnemy.OnBossAttack = Phases;
        bossEnemy.OnBossHit = BossHit;
    }

    public void Phases()
    {
        HealthPresentage = bossEnemy.GetHealtHPresentage();
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

    public virtual void BossHit(int hitIndex, int numberOfHits)
    {
        if (GuiManager.IsTrasnmiting() || bossEnemy.DelayAttack > 0)
        {
            return;
        }

        GameEventSystem.BossHitEvent?.Invoke(bossEnemy.ID, bossEnemy, .05f);

        hitIndex++;

        if (hitIndex > numberOfHits)
        {
            hitIndex = 0;
            bossAI.ChangeWaypoint(hitIndex);
        }
    }
}
