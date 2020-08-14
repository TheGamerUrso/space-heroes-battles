using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    protected bool Loop;

    protected BaseEnemy enemy;
    protected Animator animator;
    protected Coroutine EnterCoroutine;

    [Header("Enemy AI Config")]
    protected Vector3 startingPosition;
    protected Vector3 dist;
    protected Vector2 MaxScreenBound;
    protected Vector2 MinScreenBound;
    protected float timer;
    protected float delay = .5f;
    protected Vector3 movement;


    protected float speed;
    public float Speed { get { return speed; } set { speed = value; } }
    public virtual void OnEnable() { }

    public virtual void Awake()
    {
        enemy = GetComponent<BaseEnemy>();
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void Start(){}

  

    public virtual void Update() { 

    
    }

    private void LateUpdate()
    {
        Move();
    }

    public virtual void Move()
    {
        movement = (transform.forward * Speed) + (transform.right * (Speed/2));
        transform.position += movement * Time.deltaTime;
        CheckOutOfSight();
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
    public virtual void EnableMovement()
    {

    }

}