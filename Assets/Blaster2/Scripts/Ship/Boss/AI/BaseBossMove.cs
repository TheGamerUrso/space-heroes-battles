using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossMove : EnemyMove
{
    #region Boss AI Config
    [Header("Boss AI Config")]
    protected BaseBossEnemy baseBoss;
    #endregion

    #region Waypoint Config
    [Header("Waypoint Config")]
    public float cooldown;

    [SerializeField] protected GameObject WaypointPrefab;

    protected bool AutoChangeWaypoint;

    protected float moveNextPositionTimer = 2;
    #endregion
    public override void OnEnable()
    {
        baseBoss.EnableColliders(false);
    }

    public override void Awake()
    {
        base.Awake();
        baseBoss = GetComponent<BaseBossEnemy>();
    }   

    public override void Start()
    {
        speed = GetComponent<BaseEnemy>().Speed;
    }

    public virtual void ChangeWaypointByIndex(int currentPointToFollowIndex)
    {
    }

    public virtual void ChangeWaypoint(int hitIndex)
    {
    }


}
