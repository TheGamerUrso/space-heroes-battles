using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public enum EnemyMoveType
    {
        SimpleMove,FollowPath, ManeuverEnemy,Crawler,Boxer,Boss1,Boss4
    }

    public EnemyMoveType moveType;

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

    #region ManeuverEnemy AI Config
    [Header("ManeuverEnemy AI Config")]
    protected int Direction;
    protected bool directionChanged;
    float maneuverTimer = 2;
    #endregion

    #region FollowPath AI Config
    [Header("FollowPath AI Config")]
    public int[] PathIndex;
    public Transform[] Path;
    public int currentPointToFollowIndex;

    [SerializeField] public bool PingPong = false;
    [SerializeField] protected bool RotateTowardDir = false;
    [SerializeField] public bool Reset = false;
    [SerializeField] protected float RotationSpeed;

    protected Vector3 targetRotation;

    [SerializeField] protected bool reverse;
    [SerializeField] protected bool Auto;

    private Vector3 newPos;
    private float step;
    private int curPath;
    private GameObject path;
    protected float pathMagnitude;
    #endregion


    public virtual void OnDestroy() { }
    public virtual void OnEnable() {
        if (moveType == EnemyMoveType.FollowPath)
        {
            currentPointToFollowIndex = 0;

            if (Path.Length > 0)
                transform.position = Path[0].position;
        }
    }

    public virtual void Awake()
    {
        enemy = GetComponent<Enemy>();
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void Start(){
        if(moveType == EnemyMoveType.FollowPath)
        {
            startingPosition = transform.position;

            if (Path.Length == 0)
            {
                GeneratePath();
            }
        }
    } 

    public virtual void Update() { }

    public virtual void LateUpdate()
    {
        if (moveType == EnemyMoveType.SimpleMove)
        {
            movement = (transform.forward * Speed) + (transform.right * (Speed / 2));
            transform.position += movement * Time.deltaTime;
            CheckOutOfSight();
        }

        if(moveType == EnemyMoveType.FollowPath)
        {
            if (Path.Length > 0)
            {
                pathMagnitude = (Path[currentPointToFollowIndex].position - transform.position).magnitude;
                if (pathMagnitude < 2)
                {
                    if (currentPointToFollowIndex == Path.Length - 1)
                    {
                        if (Reset && !PingPong)
                        {
                            Vector3 startDir = (startingPosition - transform.position).normalized;
                            transform.position += startDir * speed * Time.deltaTime;
                            currentPointToFollowIndex = 0;
                        }
                        else if (!Reset && PingPong)
                        {
                            reverse = true;
                        }
                        else if (!Reset && !PingPong)
                        {
                            Leave();
                        }
                    }
                    else if (currentPointToFollowIndex == 0)
                    {
                        reverse = false;
                    }

                    if (Auto)
                    {
                        if (reverse)
                        {
                            currentPointToFollowIndex--;
                        }
                        else if (reverse == false)
                        {
                            currentPointToFollowIndex++;
                        }
                    }
                }

                if (currentPointToFollowIndex >= Path.Length)
                {
                    currentPointToFollowIndex = Path.Length - 1;
                }

                newPos = Path[currentPointToFollowIndex].position;

                newPos.y = 0;

                Path[currentPointToFollowIndex].position = newPos;


                Vector3 direction = (newPos - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, newPos);

                if (distance > 1)
                {
                    transform.position += direction * speed * Time.deltaTime;
                }

                if (RotateTowardDir)
                {
                    step = RotationSpeed * Time.deltaTime;
                    targetRotation = Vector3.Lerp(targetRotation, direction, step);
                    transform.rotation = Quaternion.LookRotation(targetRotation, Vector3.up);
                }
            }
        }

        if(moveType == EnemyMoveType.ManeuverEnemy)
        {
            maneuverTimer -= Time.deltaTime;

            if (maneuverTimer <= 0)
            {
                maneuverTimer = UnityEngine.Random.Range(2, 4);
                Maneuver();
            }


            movement = (transform.forward * Speed) + (transform.right * (Speed / 2));
            movement.x *= Direction;
            transform.position += movement * Time.deltaTime;


            if (transform.position.x > Constants.m_XMax)
            {
                Direction = 1;
            }
            else if (transform.position.x < Constants.m_XMin)
            {
                Direction = -1;
            }

            CheckOutOfSight();
        }
    }

    public void Leave()
    {        
        Enemy.EnemiesCount--;
        enemy.ExitLevel();
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


    #region Follow Path Enemy Type Methods
    /**
     * Follow Path Methods
     * 
     */
    public void GeneratePathByIndex(int Index)
    {
        curPath = Index;

        PingPong = false;
        if (curPath == 0)
        {
            PingPong = true;
        }

        if (Waypoints.Instance == null)
        {
            return;
        }

        path = Waypoints.Instance.GetPath(PathIndex[curPath]);
        Transform[] PathList = path.transform.GetChildrenAsList();

        GeneratePath(PathList);


        currentPointToFollowIndex = 0;
        Reset = false;
        Auto = true;

        transform.position = Path[0].position;
    }

    public int GeneratePath()
    {
        curPath = Random.Range(0, PathIndex.Length);

        if (Waypoints.Instance == null)
        {
            return -1;
        }

        path = Waypoints.Instance.GetPath(PathIndex[curPath]);
        Transform[] PathList = path.transform.GetChildrenAsList();

        GeneratePath(PathList);


        currentPointToFollowIndex = 0;
        Reset = false;
        Auto = true;

        transform.position = Path[0].position;
        return curPath;
    }

    public void GeneratePath(Transform[] newPath)
    {
        Path = newPath;
    }
    #endregion

    #region Maneuver Enemy Type Methods
    private void Maneuver()
    {
        int random = UnityEngine.Random.Range(0, 100);

        if (random <= 33.33)
        {
            Direction = 1;
        }
        else if (random > 33.33 && random <= 66.66)
        {
            Direction = -1;
        }
        else
        {
            Direction = 0;
        }

        Direction = 0;
    }
    #endregion
}