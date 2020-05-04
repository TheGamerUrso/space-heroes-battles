using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    [Header("Enemy AI Config")]

    protected BaseEnemy enemy;
    protected Rigidbody rigid;
    protected Animator animator;
    protected Coroutine EnterCoroutine;

    protected Vector3 startingPosition;
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

    protected int enterNameHash = Animator.StringToHash("Enter");
    protected int deathNameHash = Animator.StringToHash("Death");

    public void OnEnable()
    {
        Enter();
    }

    private void Awake()
    {
        InitIfNeeded();

    }

    public void Start() { Setup(); }

    public virtual void Setup(){}

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

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
      
    }

    private void LateUpdate()
    {
        Move();
    }

    public virtual void Move(){}

    public virtual void Enter(){}

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
    public virtual void EnableMovement()
    {

    }
}
