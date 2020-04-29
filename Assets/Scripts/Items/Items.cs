using UnityEngine;

public class Items : MonoBehaviour
{
    public string id;
    public string ID
    {
        get { return id; }
    }

    protected int frameInterval = 1;
    [SerializeField] protected ItemData itemData;
    public LayerMask playerLayer;

    protected static string CollectKey = "Collect";
    protected static string ResetKey = "Reset";
    [SerializeField] protected Animator animator;

    public BoxCollider boxCollider;

    protected float m_XVel;
    protected float m_YVel;
    protected float m_ZVel;

    [SerializeField] protected Vector2 m_RandomXVelValues = new Vector2();
    [SerializeField] protected Vector2 m_RandomZVelValues = new Vector2();


    protected float magnetPower;
    protected float magnetDistance = 25;


    PlayerData playerData;
    PlayerShipData playerShipData;

    private void OnEnable()
    {
        animator.SetTrigger(ResetKey);
        boxCollider.enabled = true;
    }

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        if (itemData.m_RewardAmount > 0)
        {
            if (playerShipData.MagnetPower> 0)
            {
                magnetPower = playerShipData.MagnetPower;
                magnetDistance = playerShipData.MagnetDistance;
            }
        }

        int dir = Random.Range(-1, 1);

        m_XVel = dir * Random.Range(m_RandomXVelValues.x, m_RandomXVelValues.y);
        m_ZVel = -Random.Range(m_RandomZVelValues.x, m_RandomZVelValues.y);
    }

    public void Update()
    {
        Movement();

        if (transform.position.z < Constants.m_ZMin)
        {
            gameObject.SetActive(false);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, magnetDistance);
    }

    public void DestroyNow()
    {
        gameObject.SetActive(false);
    }

    public void Movement()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, magnetDistance, playerLayer);
        if (colliders.Length > 0 && magnetPower > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider item = colliders[i];
                if (item.CompareTag("Player"))
                {
                    GameObject target = item.gameObject;
                    if (magnetPower > 0)
                    {
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.localPosition, magnetPower * Time.deltaTime);
                    }
                }
            }
        }
        else
        {
            Vector3 movement = (-transform.forward * m_ZVel) + (-transform.right * m_XVel);

            Vector3 newPos = transform.localPosition;
            newPos += movement * Time.deltaTime;
            transform.localPosition = newPos;

            if (transform.position.x < Constants.m_XMin || transform.position.x > Constants.m_XMax)
            {
                m_XVel *= -1;
            }
        }

        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, Constants.m_XMin, Constants.m_XMax), transform.localPosition.y, transform.localPosition.z);
    }

}