using UnityEngine;

public abstract class BossEnemyMovement : BaseEnemyMovement
{
    [Header("Boss Entry Settings")]
    [SerializeField] protected Vector3 entryTargetPosition = new Vector3(0, 0, 100);
    [SerializeField] protected float entrySpeed = 5f;

    protected Enemy enemyShip;
    
    [SerializeField] protected float speedByPhase;

    [SerializeField] protected Vector3[] Positions;
    [SerializeField] protected int currentPos;
    [SerializeField] protected float changePositionTimer;
    protected Vector3 targetPosition;
    protected bool RandomMovement;

    private void OnEnable()
    {
        enemyShip = ship as Enemy;
    }
  

    public void Awake()
    {
        targetPosition = transform.position;
        ((BossEnemy)ship).GetComponent<BossEnemy>().OnBossPhaseChanged += OnBossPhaseChangedHandled;
    }

    public void OnDestroy()
    {
        ((BossEnemy)ship).OnBossPhaseChanged -= OnBossPhaseChangedHandled;
    }

    public override void Update()
    {
        if (enemyShip == null) return;

        switch (enemyShip.EnemyState)
        {
            case EnemyState.Enter:
                HandleEntry();
                break;
            case EnemyState.Combat:
                Move();
                break;
            case EnemyState.Escape:
                break;
            case EnemyState.Death:
                break;
        }
    }
    protected virtual void HandleEntry()
    {
        // Move from off-screen spawn to the combat entry position
        transform.position = Vector3.MoveTowards(transform.position, entryTargetPosition, entrySpeed * Time.deltaTime);

        // Once destination is reached, switch the centralized state to Combat
        if (enemyShip.EnemyState == EnemyState.Combat)
        {
            OnCombatStarted();
        }
    }
    protected virtual void OnCombatStarted() { }

    public virtual void OnBossPhaseChangedHandled(int Phase)
    {
        if (Phase == 2)
        {
            speed += .1f;
        }
    }
}
