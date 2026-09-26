using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossEnemyMove : EnemyMove
{
    protected Vector3 targetPosition;
    
    #region Movement
    [SerializeField] private Vector3[] Positions;
    [SerializeField] private int currentPos;
    [SerializeField] private bool StartBattle;
    [SerializeField] private float changePositionTimer;
    #endregion

     #region Crawler Movement
    [Header("Crawler Settings")]
    public bool CurclularMove;
    public float angle = 0;
    public float radius = 5;
    #endregion

    #region Boxer
    [Header("Boxer Settings")]
    public bool attacking;
    public GameObject ShipPivot;
    private float cooldown;
    #endregion


}
