using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossBattle : MonoBehaviour
{
    [SerializeField] protected BaseBossEnemy bossEnemy;
    [SerializeField] protected EnemyMove bossMove;
    [SerializeField] protected float HealthPresentage;
    [SerializeField] protected int phase;
    public float DelayAttack;

    private void OnDestroy()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        bossEnemy.OnBossAttack -= Phases;
    }

    private void Start()
    {
        OnStart();
    }

    public virtual void OnStart()
    {
        bossMove = GetComponent<EnemyMove>();
        bossEnemy = GetComponent<BaseBossEnemy>();
        bossEnemy.OnBossAttack = Phases;
        bossEnemy.OnBossHit = BossHit;
    }

    public virtual void Phases()
    {
    
    }

    public virtual void BossHit(int hitIndex, int numberOfHits)
    {
    }
}
