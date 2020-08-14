using System.Collections;
using UnityEngine;

public class BattleShipMove : EnemyMove
{
    public BaseBossEnemy BossEnemy;
    public Vector3[] Positions;
    public int currentPos;
    public bool StartBattle;
    public float changePositionTimer;

    public override void Start()
    {
        StartCoroutine(DelayStart());
    }

    public override void LateUpdate()
    {
        if (StartBattle)
        {
            transform.position = Vector3.Lerp(transform.position, Positions[currentPos], speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, Positions[currentPos]) < 1)
            {
                changePositionTimer = UnityEngine.Random.Range(2, 4);
                currentPos++;
                if (currentPos > 3)
                {
                    currentPos = 0;
                }
            }
        }
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(4);

        BossEnemy.EnableAllWeapon();
        BossEnemy.EnableColliders(true);
        BossEnemy.HealthBar.Show();
        StartBattle = true;
    }


}