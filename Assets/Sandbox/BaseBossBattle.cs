using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossBattle : MonoBehaviour
{
    public BaseBossEnemy bossEnemy;
    public BaseBossEnemyAI bossAI;
    protected int phase;
    private void OnDestroy()
    {
        bossEnemy.OnBossAttack -= Phases;
    }
    private void Start()
    {
        bossAI = GetComponent<BaseBossEnemyAI>();
        bossEnemy = GetComponent<BaseBossEnemy>();
        bossEnemy.OnBossAttack = Phases;
    }

    public void Phases()
    {
        if (phase == 0 && bossEnemy.GetHealtHPresentage() <= 70f)
        {
            phase = 1;
            bossEnemy.SetFireRate();
        }
        else if (phase == 1 && bossEnemy.GetHealtHPresentage() <= 30f)
        {
            phase = 2;
            bossEnemy.SetFireRate();
        }
        else if (phase == 2 && bossEnemy.GetHealtHPresentage() <= 10f)
        {
            phase = 3;
            bossEnemy.SetFireRate();
        }
    }
}
