using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossEnemyAI : FollowPathAI
{
    #region Boss AI Config
    [Header("Boss AI Config")]
    protected BaseBossEnemy baseBoss;

    [SerializeField] protected GameObject WaypointPrefab;
    [SerializeField] protected bool m_IsMovingVertical = false;
    #endregion

    #region Waypoint Config
    [Header("Waypoint Config")]
    public float cooldown;

    protected Transform[] m_Waypoints = new Transform[3];

    protected GameObject waypointsGameObject;
    protected bool AutoChangeWaypoint;
    protected float moveNextPositionTimer = 2;

    public Vector3[] Positions;
    private int currentPath;
    #endregion

    #region Curcular Movement Settings
    [Header("Curcular Movement Settings")]
    public bool CurclularMove;
    public float angle = 0;
    float speed = (2 * Mathf.PI) / 5; //2*PI in degress is 360, so you get 5 seconds to complete a circle
    public float radius = 5;
    #endregion

   
    public override void Enter()
    {
        baseBoss.EnableColliders(false);

        if (CurclularMove)
        {
            transform.position = new Vector3(11.35104f, -50, 116.9904f);
            int changeNum = Random.Range(0, 100);
            if(changeNum >= 50)
            {
                speed *= -1;
            }       
        }
    }

    public override void Initialize()
    {
        baseBoss = GetComponent<BaseBossEnemy>();
        m_XVel = GetComponent<BaseEnemy>().GetShipStatsSystem().Speed;

        if (!CurclularMove)
        {
            List<Transform> newList = new List<Transform>();

            waypointsGameObject = Instantiate(WaypointPrefab, Vector3.zero, Quaternion.identity);
            waypointsGameObject.transform.SetParent(transform.parent);

            for (int i = 0; i < waypointsGameObject.transform.childCount; i++)
            {
                newList.Add(waypointsGameObject.transform.GetChild(i));
            }


            GeneratePath(newList.ToArray());
        }
    }

    public override void Move()
    {
        Animator animator = enemy.GetAnimator();
        var info = animator.GetCurrentAnimatorStateInfo(0);


        if (info.shortNameHash == enterNameHash)
        {
            Debug.Log("Entering");
            return;
        }

        if (baseBoss.CurrentHealth > 0)
        {
            baseBoss.EnableColliders(true);

            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }

            if (AutoChangeWaypoint)
            {
                moveNextPositionTimer -= Time.deltaTime;
                if (moveNextPositionTimer < 0)
                {
                    moveNextPositionTimer = Random.Range(2, 4);
                    ChangeWaypoint(0);
                }
            }

            if (CurclularMove)
            {
                CircularMovement();
            }
            else
            {
                base.Move();
            }
        }
    }

    public void CircularMovement()
    {
        



        angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
        
        xVel = Mathf.Cos(angle) * radius + .8f;
        zVel = Mathf.Sin(angle) * radius + 100;

        Vector3 newPos = transform.position;
        newPos.x = xVel;
        newPos.z = zVel;
        transform.position = newPos;
    }

    public void ChangeWaypointByIndex(int currentPointToFollowIndex)
    {
        this.currentPointToFollowIndex = currentPointToFollowIndex;
    }

    public virtual void ChangeWaypoint(int hitIndex)
    {
        if (!AutoChangeWaypoint)
        {
            if (cooldown <= 0)
            {
                if (!m_IsMovingVertical)
                {
                    m_IsMovingVertical = true;
                    StartCoroutine(MoveVerticalWithDelay(hitIndex));
                }
            }
        }
    }

    public virtual IEnumerator MoveVerticalWithDelay(int hitIndex)
    {
        int waitTime = Random.Range(2, 4);
        while (pathMagnitude > 1)
        {
            yield return new WaitForSeconds(waitTime);
        }

        if (currentPointToFollowIndex == 1)
        {
            currentPointToFollowIndex = 2;
        }
        else if (currentPointToFollowIndex == 2)
        {
            currentPointToFollowIndex = 1;
        }
        else
        {
            currentPointToFollowIndex = Random.Range(1, Path.Length);
        }

        yield return new WaitForSeconds(1);
        m_IsMovingVertical = false;
        cooldown = UnityEngine.Random.Range(4, 6);
        hitIndex = 0;
    }

}
