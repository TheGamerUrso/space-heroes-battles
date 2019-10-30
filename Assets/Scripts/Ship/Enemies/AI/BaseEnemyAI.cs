using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    [Header("Simple AI Config")]

    protected int Direction;
    protected bool Loop;
    protected bool directionChanged;
    protected Vector3 startingPosition;
    protected Animator animator;
    protected Coroutine EnterCoroutine;
    protected Vector3 dist;
    protected Vector2 MaxScreenBound;
    protected Vector2 MinScreenBound;
    protected float timer;
    protected float delay = .5f;
    protected Vector3 movement;

    protected Enemy enemy;
    protected Rigidbody rigid;

    protected float m_XVel;
    protected float m_ZVel;

    protected float XVel
    {
        get
        {
            return m_XVel;
        }
        set
        {
            m_XVel = value;
        }
    }

    protected float ZVel
    {
        get
        {
            return m_ZVel;
        }
        set
        {
            m_ZVel = value;
        }
    }

    private void OnEnable()
    {
        Enter();
    }

    public void Start()
    {
        InitIfNeeded();
        Initialize();

        XVel = enemy.GetShipStatsSystem().GetSpeed() / 2;
        ZVel = enemy.GetShipStatsSystem().GetSpeed();

        Direction = 0;
    }

    public virtual void Initialize()
    {
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
            enemy = GetComponent<Enemy>();
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

    public void Update()
    {
        Move();
    }

    public virtual void Move() { }

    public virtual void Enter()
    {
        if (EnterCoroutine != null)
        {
            StopCoroutine(EnterAnimationCoroutine());
        }

        StartCoroutine(EnterAnimationCoroutine());
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
