using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWidget : MonoBehaviour
{
    private Player player;

    private PlayerWeaponSystem playerWeaponSystem;

    private static string ReadyStringKey = "Ready";
    private static string ActiveStringKey = "Active";

    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color zeroHealthColor = Color.red;

    [Space(2)]
    [SerializeField] private GameObject SuperWidget;

    [SerializeField] private GameObject HealthWidget;

    [Space(2)]
    [SerializeField] private Slider XPBar = null;

    [Space(2)]
    [SerializeField] private TextMeshProUGUI XPStatus;

    [SerializeField] private TextMeshProUGUI HealthText = null;
    [SerializeField] private Button PowerBut;
    [SerializeField] private Image m_HealthImage = null;
    [SerializeField] private Image m_PowerUps;
    [SerializeField] private Image WeaponIndicatorImage;
    [SerializeField] private Image m_ShieldImage;

    [Space(2)]
    [Range(1, 4)] private int CurrentWeapnType = 0;

    // private int WeaponUpgradeCollected = 0;

    [Space(2)]
    [SerializeField] private Animator PowerUIActiveAnimator = null;

    [Space(2)]
    [SerializeField] private Sprite[] WeaponIndicatorSpritesActivated = null;

    [SerializeField] private Sprite[] WeaponIndicatorSpritesNotActivated;

    private float timer;
    private float targetHealth = 0;
    private float maxTargetHealth = 0;


    public void SetPlayer(Player player)
    {
        playerWeaponSystem = this.player.GetWeaponSystem();

        PowerBut.onClick.AddListener(() =>
        {
            if (playerWeaponSystem)
            {
              
                playerWeaponSystem.ActivateSpecial();
            }
        });


    }

    public void ActivateSpecial()
    {
        PowerBut.interactable = false;
        PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
    }

    private void Update()
    {
        if (player == null)
        {
            if (PlayerManager.GetPlayer() == null)
            {
                return;
            }

            player = PlayerManager.GetPlayer();
            if (player != null)
            {
                SetPlayer(player);
            }
            else
            {
                return;
            }
        }

        if (Time.frameCount % 1 == 0)
        {
            float playerPowerUp = 0;

            playerPowerUp = playerWeaponSystem.GetPowerUpLevelPresentage();

            if (playerPowerUp >= 1)
            {
                if (!PlayerPrefs.HasKey("SuperTut"))
                {
                    Tutorial.Instance.ShowTutorial(4);

                    PlayerPrefs.SetInt("SuperTut", 1);
                }
                PowerBut.interactable = true;
                PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
            }
            else
            {
                PowerBut.interactable = false;
                PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
            }

            m_PowerUps.fillAmount = playerPowerUp;

            if (m_PowerUps.fillAmount == 1)
            {
                RefreshWeaponIndicatorSprite();
            }

            if (m_PowerUps.fillAmount < 1)
            {
                RefreshWeaponIndicatorSprite();
            }



#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.T))
            {
                PlayerWeaponSystem playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
                playerWeaponSystem.IncreasePowerUp(0.5f);
                playerWeaponSystem.WeaponPowerUPCollected();
            }
#endif


            LevelSystem levelSystem = player.GetLevelSystem();
            UpdateXP(levelSystem);
            UpdatePlayerHealth(player.CurrentHealth, player.MaxHealth);
        }
    }

    public void UpdateXP(LevelSystem levelSystem)
    {
        XPBar.maxValue = levelSystem.GetXpToLevel();
        XPBar.value = levelSystem.GetXP();
        XPStatus.text = string.Format("{0}/{1}", levelSystem.GetXP(), levelSystem.GetXpToLevel());
    }

    public void UpdatePlayerHealth(float CurrentHealth, float MaxHealth)
    {
        HealthText.text = string.Format("{0}/{1}", Mathf.Round(CurrentHealth), MaxHealth);
        m_HealthImage.fillAmount = (CurrentHealth / MaxHealth);
        maxTargetHealth = MaxHealth;
        targetHealth = CurrentHealth;

        m_HealthImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, targetHealth / maxTargetHealth);
    }

    public void ShieldEffect(bool value)
    {
        m_ShieldImage.gameObject.SetActive(value);
    }

    public void RefreshWeaponIndicatorSprite()
    {
        var sprite = WeaponIndicatorSpritesNotActivated[0];
        
        var collecterUpgrade = PlayerWeaponSystem.WeaponUpgradeCollected;
        if (m_PowerUps.fillAmount == 1 && collecterUpgrade >= WeaponIndicatorSpritesNotActivated.Length)
        {
            sprite = WeaponIndicatorSpritesActivated[PlayerWeaponSystem.WeaponUpgradeCollected];
        }
        else if(collecterUpgrade >= WeaponIndicatorSpritesNotActivated.Length)
        {
            sprite = WeaponIndicatorSpritesNotActivated[PlayerWeaponSystem.WeaponUpgradeCollected];
        }
        else
        {
            sprite = WeaponIndicatorSpritesNotActivated[PlayerWeaponSystem.WeaponUpgradeCollected];
        }

        WeaponIndicatorImage.sprite = sprite;
    }


}