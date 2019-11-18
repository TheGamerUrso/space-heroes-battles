using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    [Header("Enemy AI Config")]

    protected BaseEnemy enemy;
    protected Rigidbody rigid;

    protected int Direction;
    protected bool Loop;
    protected bool directionChanged;
    protected Vector3 startingPosition;
    protected Coroutine EnterCoroutine;
    protected Vector3 dist;
    protected Vector2 MaxScreenBound;
    protected Vector2 MinScreenBound;
    protected float timer;
    protected float delay = .5f;
    protected Vector3 movement;

    protected float m_XVel;
    protected float m_ZVel;

    public float xVel { get { return m_XVel; } set { m_XVel = value; } }
    public float zVel { get { return m_ZVel; } set { m_ZVel = value; } }

    public bool bAppeared, bEntered;

    [Header("MinerBossAI Config")]
    protected int enterNameHash = Animator.StringToHash("Enter");
    protected int deathNameHash = Animator.StringToHash("Death");

    private void OnEnable()
    {
        Appear();
    }

    private void Awake()
    {
        InitIfNeeded();
        Initialize();
    }

    public void Start()
    {
        m_XVel = enemy.GetShipStatsSystem().GetSpeed() / 2;
        m_ZVel = enemy.GetShipStatsSystem().GetSpeed();
        
        Enter(); 

        Direction = 0;
    }

    public virtual void Initialize()
    {
        Debug.Log(gameObject.name);
        startingPosition = transform.position;
    }

    public virtual void InitIfNeeded()
    {
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody>();
        }

        if (enemy == null)
        {
            enemy = GetComponent<BaseEnemy>();
        }
    }

    public IEnumerator EnterAnimationCoroutine()
    {
        while (transform.GetChild(1).transform.localScale.x < 1)
        {
            Vector3 NewSize = transform.GetChild(1).transform.localScale;
            NewSize.x += Time.deltaTime;
            NewSize.y += Time.deltaTime;
            NewSize.z += Time.deltaTime;
            if (NewSize.x > 1)
            {
                NewSize = Vector3.one;
            }
            transform.GetChild(1).transform.localScale = NewSize;
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public virtual void Move() { }

    public virtual void Appear()
    {
        if (EnterCoroutine != null)
        {
            StopCoroutine(EnterAnimationCoroutine());
        }

        StartCoroutine(EnterAnimationCoroutine());
    }
    public virtual void Enter(){     
    }

    public void Leave()
    {
        enemy.Leave();
        gameObject.SetActive(false);
    }

    public void CheckOutOfSight()
    {
        if (transform.position.z < Constants.m_ZMin)
        {
            Leave();
        }
    }
}
