using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerMove : EnemyMove
{
    public BaseBossEnemy BossEnemy;

    public bool StartBattle;


    public bool CurclularMove;
    public float angle = 0;
    public float radius = 5;

    public override void OnEnable()
    {
        int changeNum = Random.Range(0, 100);
        if (changeNum >= 50)
        {
            speed *= -1;
        }
    }

    public override void Start()
    {
        StartCoroutine(DelayStart());
        speed = (2 * Mathf.PI) / 5; //2*PI in degress is 360, so you get 5 seconds to complete a circle
    }


    public override void EnableMovement()
    {
        CurclularMove = true;
    }

    public override void LateUpdate()
    {
        if (StartBattle)
        {
            if (CurclularMove)
            {
                CircularMovement();
            }
        }
    }

    public void CircularMovement()
    {
        angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=

        var xVel = Mathf.Cos(angle) * radius + .8f;
        var zVel = Mathf.Sin(angle) * radius + 100;

        Vector3 newPos = transform.position;
        newPos.x = xVel;
        newPos.z = zVel;
        transform.position = Vector3.MoveTowards(transform.position, newPos, .5f);
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
