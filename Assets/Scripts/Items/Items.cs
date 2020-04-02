using UnityEngine;

public class Items : MonoBehaviour
{
    private int frameInterval = 1;
    [SerializeField] private ItemData itemData;
    public LayerMask playerLayer;

    private static string CollectKey = "Collect";
    private static string ResetKey = "Reset";
    public Animator animator;

    public BoxCollider boxCollider;

    private float m_XVel;
    private float m_YVel;
    private float m_ZVel;

    [SerializeField] private Vector2 m_RandomXVelValues = new Vector2();
    [SerializeField] private Vector2 m_RandomZVelValues = new Vector2();


    private float magnetPower;
    private float magnetDistance = 25;

    public ItemData GetItemType()
    {
        return itemData;
    }

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
        PlayerShip player = PlayerManager.GetPlayer();

        if (itemData.m_RewardAmount > 0 && player.GetMagnetPower() > 0)
        {
            magnetPower = 10 + player.GetMagnetPower();
            magnetDistance = 25 + player.GetMagnetDistanceUpgrade();
        }

        int dir = Random.Range(-1, 1);

        m_XVel = dir * Random.Range(m_RandomXVelValues.x, m_RandomXVelValues.y);
        m_ZVel = -Random.Range(m_RandomZVelValues.x, m_RandomZVelValues.y);
    }

    public void Update()
    {
        if (Time.frameCount % frameInterval == 0)
        {
            Movement();

            if (transform.position.z < Constants.m_ZMin)
            {
                gameObject.SetActive(false);

                //  Destroy(gameObject);
            }
        }
    }

    public void Action(PlayerShip player)
    {
        if (boxCollider)
        {
            boxCollider.enabled = false;
        }

        animator.SetTrigger(CollectKey);

        if (itemData.m_HealValue > 0)
        {
            player.Heal(player.Level * itemData.m_HealValue);
        }

        if (itemData.m_RewardAmount > 0)
        {
            //TODO Increase Coin Earn In Game
  
            GameSession.counsEarnInGame++;
        }

        if (itemData.Shield)
        {
            player.InstallShieldModule();
        }

        if (itemData.PowerPack)
        {
            player.PowerUpCollected();
        }

        if (AudioManager.Instance)
        {
            AudioManager.PlaySound(null, itemData.CollectedSoundSFX, 2);
        }
        Invoke("SetGameObjectOff", 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, magnetDistance);
    }

    public void Movement()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, magnetDistance, playerLayer);
        if (colliders.Length > 0)
        {
            foreach (Collider item in colliders)
            {
                if (item.tag.Equals("Player"))
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

            transform.localPosition += movement * Time.deltaTime;

            if (transform.position.x < Constants.m_XMin || transform.position.x > Constants.m_XMax)
            {
                m_XVel *= -1;
            }
        }

        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, Constants.m_XMin, Constants.m_XMax), transform.localPosition.y, transform.localPosition.z);
    }

}