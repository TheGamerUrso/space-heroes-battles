using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Utils;
using UnityEngine;

public class FirstBossAI : BaseBossEnemyAI
{
    #region FollowPath AI Config
    [Header("FollowPath AI Config")]
    private Transform[] Path;
    public int currentPointToFollowIndex;

    [SerializeField] protected bool PingPong = false;
    [SerializeField] protected bool RotateTowardDir = false;
    [SerializeField] public bool Reset = false;
    [SerializeField] protected float RotationSpeed;

    protected Vector3 targetRotation;

    [SerializeField] protected bool reverse;
    [SerializeField] protected bool SpawnAtFirstPath;
    [SerializeField] protected bool Auto;
    private Vector3 newPos;
    private float step;
    private int curPath;
    private Vector3 dir;
    private Transform[] PathList;
    private GameObject path;
    protected float pathMagnitude;
    #endregion


    [SerializeField] protected GameObject WaypointPrefab;
    [SerializeField] protected bool m_IsMovingVertical = false;

    public void GeneratePath(Transform[] newPath)
    {
        Path = newPath;
    }

    public override void Setup()
    {
        base.Setup();

        m_ZVel = baseBoss.GetShipStatsSystem().Speed;

        startingPosition = transform.position;

        List<Transform> newList = new List<Transform>();

        waypointsGameObject = Instantiate(WaypointPrefab, Vector3.zero, Quaternion.identity);
        waypointsGameObject.transform.SetParent(transform.parent);

        for (int i = 0; i < waypointsGameObject.transform.childCount; i++)
        {
            newList.Add(waypointsGameObject.transform.GetChild(i));
        }
        GeneratePath(newList.ToArray());

    }

    public override void Enter()
    {
        base.Enter();
        currentPointToFollowIndex = 0;
    }


    public override void Move()
    {
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

            newPos = Path[currentPointToFollowIndex].position;

            newPos.y = -50;

            Path[currentPointToFollowIndex].position = newPos;


            Vector3 direction = (newPos - transform.position);
            Vector3 normalizedDirection = direction.normalized;
            float distance = direction.magnitude;

            if (distance > 1)
            {
                transform.position += normalizedDirection * m_ZVel * Time.deltaTime;
            }
        }
    }

    public override void ChangeWaypointByIndex(int currentPointToFollowIndex)
    {
        this.currentPointToFollowIndex = currentPointToFollowIndex;
    }

    public override void ChangeWaypoint(int hitIndex)
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

    public IEnumerator MoveVerticalWithDelay(int hitIndex)
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