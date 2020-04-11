using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWidget : MonoBehaviour
{
    private PlayerShip player;

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

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdatePlayerHealth;
            player.PowerUpLevelChanged -= PowerUpLevelChanged;
            player.OnXpChanged -= UpdateXP;
        }
    }

    public void SetPlayer(PlayerShip player)
    {
        this.player = player;

        PowerBut.onClick.AddListener(() =>
        {
                ActivateSpecial();
            
        });

        player.OnHealthChanged += UpdatePlayerHealth;
        player.PowerUpLevelChanged += PowerUpLevelChanged;


        player.OnXpChanged += UpdateXP;

        UpdatePlayerHealth(player.CurrentHealth, player.MaxHealth);
      
        UpdateXP(player.level, Mathf.Abs(player.xp), player.xpToLevel);
       
        
        PowerUpLevelChanged(player.GetPowerUpLevelPresentage());
    }

    public void ActivateSpecial()
    {
        PowerBut.interactable = false;
        PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
    }


    public void PowerUpLevelChanged(float playerPowerUp)
    {
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
    }

    public void UpdateXP(int lvl, float xp, float xpToLevel)
    {
        XPBar.maxValue = xpToLevel;
        XPBar.value = xp;
        XPStatus.text = string.Format("{0}/{1}", xp, xpToLevel);
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

        var collecterUpgrade = player.PowerUpCollectAmmount;
        if (m_PowerUps.fillAmount == 1 && collecterUpgrade >= WeaponIndicatorSpritesNotActivated.Length)
        {
            sprite = WeaponIndicatorSpritesActivated[player.PowerUpCollectAmmount];
        }
        else if (collecterUpgrade >= WeaponIndicatorSpritesNotActivated.Length)
        {
            sprite = WeaponIndicatorSpritesNotActivated[player.PowerUpCollectAmmount];
        }
        else
        {
            sprite = WeaponIndicatorSpritesNotActivated[player.PowerUpCollectAmmount];
        }

        WeaponIndicatorImage.sprite = sprite;
    }


}