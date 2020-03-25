using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossEnemyAI : BaseEnemyAI
{
    #region Boss AI Config
    [Header("Boss AI Config")]
    protected BaseBossEnemy baseBoss;
    #endregion

    #region Waypoint Config
    [Header("Waypoint Config")]
    public float cooldown;

    protected GameObject waypointsGameObject;

    protected bool AutoChangeWaypoint;

    protected float moveNextPositionTimer = 2;
    #endregion


    public override void InitIfNeeded()
    {
        base.InitIfNeeded();
        baseBoss = GetComponent<BaseBossEnemy>();
    }

    public override void Enter()
    {
        baseBoss.EnableColliders(false);
    }

    public override void Setup()
    {
        m_XVel = GetComponent<BaseEnemy>().GetShipStatsSystem().Speed;
    }

    public virtual void ChangeWaypointByIndex(int currentPointToFollowIndex)
    {
    }

    public virtual void ChangeWaypoint(int hitIndex)
    {
    }

    
}
