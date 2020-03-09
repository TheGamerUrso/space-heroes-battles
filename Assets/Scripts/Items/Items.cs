using UnityEngine;

public class Items : MonoBehaviour
{
    public GameController gameController;
    private int frameInterval = 1;
    [SerializeField] private ItemData itemData;

    private static string CollectKey = "Collect";
    private static string ResetKey = "Reset";
    public Animator animator;

    public BoxCollider boxCollider;
    private GameObject model;

    private float m_XVel;
    private float m_YVel;
    private float m_ZVel;

    [SerializeField] private Vector2 m_RandomXVelValues = new Vector2();
    [SerializeField] private Vector2 m_RandomZVelValues = new Vector2();

    private Vector3 m_Rotation = new Vector3(0, 1, 0);
    private GameObject TargetToMoveTo;

    public float magnetPower;
    private float magnetDistance = 25;
    private Player player;
    bool MagnetActive;

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
        gameController = GameController.Instance;
        Initialize();
    }
    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void Initialize()
    {
        SetPlayer(PlayerManager.GetPlayer());
        TargetToMoveTo = player.gameObject;

        if (itemData.m_RewardAmount > 0 && player.GetUpgradeSystem().GetMagnetPower() > 0)
        {
            MagnetActive = true;
        }

        if (MagnetActive)
        {
            magnetPower = 10 + player.GetUpgradeSystem().GetMagnetPower();
            magnetDistance = 25 + player.GetUpgradeSystem().GetMagnetDistanceUpgrade();
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

    public void Action()
    {

        if (boxCollider)
        {
            boxCollider.enabled = false;
        }

        animator.SetTrigger(CollectKey);
        if (itemData.m_HealValue > 0)
        {
            player.Heal(player.Level * itemData.m_HealValue);
            GuiManager.Instance.CreateFloatingText("Heal up", transform.localPosition);
            if (AudioManager.Instance)
            {
                AudioManager.PlaySound(null, itemData.CollectedSoundSFX, 2);
            }
            if (!PlayerPrefs.HasKey("HealTut"))
            {
                Tutorial.Instance.ShowTutorial(1);
                PlayerPrefs.SetInt("HealTut", 1);
            }
        }

        if (itemData.m_RewardAmount > 0)
        {
            gameController.counsEarnInGame++;
            GuiManager.Instance.CreateFloatingText("$", transform.localPosition);
            if (AudioManager.Instance)
            {
                AudioManager.PlaySound(null, itemData.CollectedSoundSFX, 3);
            }

            GuiManager.Instance.UpdateCoinWidgetText();


            if (!PlayerPrefs.HasKey("CoinTut"))
            {
                Tutorial.Instance.ShowTutorial(0);
                itemData.ShowTutorial = true;
                PlayerPrefs.SetInt("CoinTut", 1);
            }
        }

        if (itemData.Shield)
        {
            player.InstallShieldModule();
            GuiManager.Instance.CreateFloatingText("Shield Up", transform.localPosition);
            if (AudioManager.Instance)
            {
                AudioManager.PlaySound(null, itemData.CollectedSoundSFX, 2);
            }
            if (!PlayerPrefs.HasKey("ShieldTut"))
            {
                Tutorial.Instance.ShowTutorial(3);
                itemData.ShowTutorial = true;
                PlayerPrefs.SetInt("ShieldTut", 1);
            }
        }

        if (itemData.PowerPack)
        {
            PlayerWeaponSystem playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
            playerWeaponSystem.WeaponPowerUPCollected();
            GuiManager.Instance.CreateFloatingText("Power Up", transform.localPosition);

            if (AudioManager.Instance)
            {
                AudioManager.PlaySound(null, itemData.CollectedSoundSFX, 2);
            }

            if (!PlayerPrefs.HasKey("PowerTut"))
            {
                Tutorial.Instance.ShowTutorial(2);
                itemData.ShowTutorial = true;
                PlayerPrefs.SetInt("PowerTut", 1);
            }



        }


        GuiManager.Instance.UpdateScore(75);

        Invoke("SetGameObjectOff", 1);
    }

    private void SetGameObjectOff()
    {
        gameObject.SetActive(false);
    }

    public void Movement()
    {
        float distance = 0;

        if (player != null)
        {
            distance = Vector3.Magnitude(transform.localPosition - TargetToMoveTo.transform.localPosition);
        }

        if (player != null && magnetPower > 0 && distance < magnetDistance)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetToMoveTo.transform.localPosition, magnetPower * Time.deltaTime);
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