using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossBattle : MonoBehaviour
{
    public BaseBossEnemy baseBossEnemy;
    protected int phase;
    private void OnDestroy()
    {
        baseBossEnemy.OnBossAttack -= Phases;
    }
    private void Start()
    {
        baseBossEnemy = GetComponent<BaseBossEnemy>();
        baseBossEnemy.OnBossAttack = Phases;
    }

    public void Phases()
    {
        if (phase == 0 && baseBossEnemy.GetHealtHPresentage() <= 70f)
        {
            phase = 1;
            baseBossEnemy.SetFireRate();
        }
        else if (phase == 1 && baseBossEnemy.GetHealtHPresentage() <= 30f)
        {
            phase = 2;
            baseBossEnemy.SetFireRate();
        }
        else if (phase == 2 && baseBossEnemy.GetHealtHPresentage() <= 10f)
        {
            phase = 3;
            baseBossEnemy.SetFireRate();
        }
    }
}
