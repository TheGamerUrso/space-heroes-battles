using TheGamerUrso.Utils;
using UnityEngine;

public class MinerBossAI : BaseBossEnemyAI
{
    [Header("MinerBossAI Config")]
    public Vector3[] Positions;
    private int currentPath;

    public override void Initialize()
    {
        base.Initialize();

        transform.position = Positions[0];
        currentPath = 1;

        m_XVel = GetComponent<Enemy>().GetShipStatsSystem().Speed;
    }



    public override void Move()
    {
        if (appear == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, Positions[currentPath], m_XVel * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 180, 0);

            if (transform.position == Positions[1])
            {
               m_XVel = 50;
                appear = true;
            }
        }
        else
        {
            if (appear)
            {
             
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 0), 1);
                Vector3 pos = Path[0].position;
                float dist = (transform.localPosition - pos).magnitude;
                if (dist < 5)
                {
                   
                    entered = true;
                }
            }
        }

        if (appear)
        {
            base.Move();
        }
    }
}