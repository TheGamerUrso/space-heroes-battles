using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattle : MonoBehaviour
{
    public enum BossBattleType
    {
        boss1,boss2,boss3,boss4
    }
    public BossBattleType bossBattleType;
    [SerializeField] protected BossEnemy bossEnemy;
    [SerializeField] protected BossEnemyMove bossMove;
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
        bossMove = GetComponent<BossEnemyMove>();
        bossEnemy = GetComponent<BossEnemy>();
        bossEnemy.OnBossAttack = Phases;
        bossEnemy.OnBossHit = BossHit;
    }

    public virtual void Phases()
    {
        switch (bossBattleType)
        {
            case BossBattleType.boss1:
                HealthPresentage = bossEnemy.GetHealthPresentage();
                if (phase == 0 && HealthPresentage <= 80f)
                {
                    phase = 1;
                }
                else if (phase == 1 && HealthPresentage <= 60f)
                {
                    phase = 2;
                }
                else if (phase == 2 && HealthPresentage <= 30f)
                {
                    phase = 3;
                }
                break;
            case BossBattleType.boss2:
                break;
            case BossBattleType.boss3:
                break;
            case BossBattleType.boss4:
                break;
            default:
                break;
        }
    }

    public void BossHit(int hitIndex, int numberOfHits)
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
        }

        switch (bossBattleType)
        {
            case BossBattleType.boss1:
          
                break;
            case BossBattleType.boss2:
                break;
            case BossBattleType.boss3:

                break;
            case BossBattleType.boss4:
                if (Random.Range(0, 100) >= 50)
                {
                    if (HealthPresentage % 40 == 0)
                    {

                    }
                }
                break;
            default:
                break;
        }
    }
}
