using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class BossEnemyMove : MonoBehaviour
{
    public enum BossEnemyType
    {
        Boss1, Crawler, Hornet, Boxer
    }

    public BossEnemyType moveType;

    [SerializeField] private Vector3[] Positions;
    [SerializeField] private int currentPos;
    [SerializeField] private bool StartBattle;
    [SerializeField] private float changePositionTimer;

    [SerializeField] private BossEnemy bossEnemy;
    [SerializeField] private Animator animator;

    private bool Loop;
    private Coroutine EnterCoroutine;

    private Vector3 startingPosition;
    private Vector3 dist;
    private Vector2 MaxScreenBound;
    private Vector2 MinScreenBound;
    [SerializeField] private float timer;
    [SerializeField] private float delay = .5f;
    private Vector3 movement;
    private Vector3 targetPosition;

    [SerializeField] private float speed;
    public float Speed { get { return speed; } set { speed = value; } }

    #region Crawler Movement
    [Header("Crawler Settings")]
    public bool CurclularMove;
    public float angle = 0;
    public float radius = 5;
    #endregion

    #region Boxer
    [Header("Boxer Settings")]
    public Punch[] punches;
    public bool attacking;
    public GameObject ShipPivot;
    private float cooldown;
    #endregion

    public virtual void OnDestroy() { }
    public virtual void OnEnable()
    {
        if (moveType == BossEnemyType.Crawler)
        {
            int changeNum = Random.Range(0, 100);
            if (changeNum >= 50)
            {
                speed *= -1;
            }
        }
    }

    public virtual void Awake()
    {
        bossEnemy = GetComponent<BossEnemy>();
        animator = GetComponentInChildren<Animator>();

        targetPosition = transform.position;
    }

    public virtual void Start()
    {
        if (moveType == BossEnemyType.Crawler)
        {
            speed = (2 * Mathf.PI) / 5; //2*PI in degress is 360, so you get 5 seconds to complete a circle
        }
        StartCoroutine(DelayStart());
    }

    public virtual void Update() {

        transform.position = Vector3.Lerp(transform.position,targetPosition,speed * Time.deltaTime);
     }

    public virtual void LateUpdate()
    {
        if (StartBattle)
        {
            if (moveType == BossEnemyType.Crawler)
            {
                CircularMovement();
            }
            else if (moveType == BossEnemyType.Boxer)
            {
                if (bossEnemy.CurrentHealth > 0)
                {
                    if (!attacking && cooldown > 0)
                    {
                        cooldown -= Time.deltaTime;
                    }

                    if (cooldown <= 0)
                    {
                        if (punches.Length > 0)
                        {
                            List<Punch> newList = punches.Where(x => x.CurrentHealth > 0).ToList();
                            int rand = UnityEngine.Random.Range(0, newList.Count);
                            if (newList.Count > 0)
                            {
                                cooldown = UnityEngine.Random.Range(4, 8);
                                newList[rand].Attack((x) =>
                                {
                                    attacking = x;
                                });
                            }
                        }
                    }

                    transform.position = Vector3.Lerp(transform.position, Positions[currentPos], speed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, Positions[currentPos]) < 1)
                    {
                        changePositionTimer = UnityEngine.Random.Range(2, 4);
                        currentPos++;
                        if (currentPos > 3)
                        {
                            currentPos = 0;
                        }
                    }
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, Positions[currentPos], speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, Positions[currentPos]) < 1)
                {
                    changePositionTimer = UnityEngine.Random.Range(2, 4);
                    currentPos++;
                    if (currentPos > 3)
                    {
                        currentPos = 0;
                    }
                }
            }
        }
    }
    IEnumerator DelayStart()
    {
        bossEnemy.HealthBar.Show();
        yield return new WaitForSeconds(4);
        bossEnemy.EnableAllWeapon();
        bossEnemy.EnableColliders(true);
        StartBattle = true;
    }

    public void Leave()
    {
        bossEnemy.Leave();
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
        if (moveType == BossEnemyType.Crawler)
        {
            CurclularMove = true;
        }
    }

    public void CircularMovement()
    {
        angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=

        var xVel = Mathf.Cos(angle) * radius + .8f;
        var zVel = Mathf.Sin(angle) * radius + 100;

        Vector3 newPos = targetPosition;
        newPos.x = xVel;
        newPos.z = zVel;
        targetPosition = newPos;
    }

}
