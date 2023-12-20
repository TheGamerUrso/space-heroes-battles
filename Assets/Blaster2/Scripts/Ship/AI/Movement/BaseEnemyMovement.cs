using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyMovement : MonoBehaviour
{
    protected Enemy enemy;
    protected Animator animator;

    protected bool Loop;
    protected Coroutine EnterCoroutine;

    protected Vector3 startingPosition;
    protected Vector3 dist;
    protected Vector2 MaxScreenBound;
    protected Vector2 MinScreenBound;
    protected float timer;
    protected float delay = .5f;
    protected Vector3 movement;

    protected float speed;
    public float Speed { get { return speed; } set { speed = value; } }

    public virtual void OnDestroy() { }
    public virtual void OnEnable() 
    {
 
    }

    public virtual void Awake()
    {
        enemy = GetComponent<Enemy>();
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void Start(){
       
    } 

    public virtual void Update() { }

    public virtual void LateUpdate()
    {
      Movement();
    }

    public virtual void Movement()
    {

    }

    public void ExitLevel()
    {
        enemy.ExitLevel();
        gameObject.SetActive(false);
    }

    public void CheckOutOfSight()
    {
        if (transform.position.z < Constants.m_ZMin)
        {
            ExitLevel();
        }
    }
    public virtual void EnableMovement()
    {

    }
}
