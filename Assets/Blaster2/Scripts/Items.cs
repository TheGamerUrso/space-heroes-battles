using TheGamerUrso.Core;
using UnityEngine;

public enum ItemEnum
{
    COIN, SHIELD, POWERUP, HEALTH, EMPTY
}

public class Items : MonoBehaviour, IPickable
{
    private PlayerShip player;

    [SerializeField] private Item_SO itemData;
    [SerializeField] private LayerMask playerLayer;
    public BoxCollider boxCollider;

    protected float m_XVel;
    protected float m_YVel;
    protected float m_ZVel;

    [SerializeField] protected Vector2 m_RandomXVelValues = new Vector2();
    [SerializeField] protected Vector2 m_RandomZVelValues = new Vector2();


    protected float magnetPower;
    protected float magnetDistance = 25;

    private bool picked;
    private float TTL = .2f;

    PlayerData playerData;
    PlayerShipData playerShipData;

    [SerializeField] protected AudioSource audioSource;
    private Collider[] colliders;
    private IAudioService audioService;

    public string ID
    {
        get { return itemData.ID; }
    }
    private IDataService dataService;

    private void OnEnable()
    {
        boxCollider.enabled = true;
        picked = false;
    }
    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();
        audioService = GameContext.Get<IAudioService>();


        PlayerData playerData = dataService.GetPlayerData();
        player = PlayerManager.GetPlayer();
    }


    private void Start()
    {
        if (itemData.itemType == ItemEnum.COIN)
        {
            if (playerShipData.MagnetPower > 0)
            {
                magnetPower = playerShipData.MagnetPower;
                magnetDistance = playerShipData.MagnetDistance;
            }
        }

        int dir = Random.Range(-1, 1);

        m_XVel = dir * Random.Range(m_RandomXVelValues.x, m_RandomXVelValues.y);
        m_ZVel = -Random.Range(m_RandomZVelValues.x, m_RandomZVelValues.y);
    }


    public void PickUp()
    {
        if (boxCollider)
        {
            boxCollider.enabled = false;
        }

        switch (itemData.itemType)
        {
            case ItemEnum.COIN:
                GameController.Instance.SetCoinPicked(itemData.ammount);
                player.SetWallet(itemData.ammount);
                break;
            case ItemEnum.SHIELD:
                player.InstallShield();
                player.OnItemPickedUp?.Invoke(0);
                break;
            case ItemEnum.POWERUP:
                player.PowerUpCollected();
                player.OnItemPickedUp?.Invoke(1);
                break;
            case ItemEnum.HEALTH:
                player.Heal(itemData.ammount * playerShipData.level);
                player.OnItemPickedUp?.Invoke(2);
                break;
            case ItemEnum.EMPTY:
                break;
            default:
                break;
        }

        audioService.PlaySound(itemData.CollectedSoundSFX);

        picked = true;
    }

    public void Update()
    {
        Movement();

        if (picked)
        {
            TTL -= Time.deltaTime;
            if (TTL <= 0)
            {
                gameObject.SetActive(false);
            }
        }


        if (transform.position.z < Constants.m_ZMin)
        {
            gameObject.SetActive(false);
        }
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