using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeElement : MonoBehaviour,IPurchasable
{


    [SerializeField] protected TextMeshProUGUI CostText;
    [SerializeField] protected TextMeshProUGUI NammeText;
    [SerializeField] protected TextMeshProUGUI NotAvailbleText;

    [SerializeField] protected Image progressBar;
    [SerializeField] protected Image UpgradeIcon;
    [SerializeField] protected Image NotAvailableImage;

    public Upgrade upgrade;

    protected PlayerData playerData;
    protected PlayerShipData playerShipData;
    protected UpgradeManager upgradeManager;

    private void OnEnable()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        RefreshUpgradeElement();
    }

    public virtual bool CheckAvailable()
    {
        if (upgrade.ReachedMaxLevel)
        {
            Popup.Show(Popup.popupType.message, Constants.UpgradeMaxedOut);
            return false;
        }

        if (upgrade.CheckRequirement(playerShipData.level))
        {
            return true;
        }
        else
        {
            Popup.Show(Popup.popupType.message, upgrade.GetLevelRequirment());
        }

        return false;
    }

    private void OnDestroy()
    {
        Events.OnLevelValueChanged -= OnLevelChanged;
        Events.OnCoinValueChanged -= OnCoinValueChanged;
    }

    private void Start()
    {
        InitUpgradeElement(UpgradeManager.Instance);

        Events.OnCoinValueChanged += OnCoinValueChanged;
        Events.OnLevelValueChanged += OnLevelChanged;
    }

    public void OnCoinValueChanged(int coins)
    {
        RefreshUpgradeElement();
    }

    public void OnLevelChanged(int level)
    {
        RefreshUpgradeElement();
    }

    public virtual void InitUpgradeElement(UpgradeManager upgradeManager)
    {
        this.upgradeManager = upgradeManager;
        upgradeManager.SubscribePurchasable(this);

        playerShipData = playerData.GetCurrentPlayerShipData();
        
        upgrade.Initialise(playerShipData);

        NammeText.text = upgrade.GetUpgradeName();


        UpgradeIcon.sprite = upgrade.UpgradeIcon;

        if (upgrade.ReachedMaxLevel)
        {
            CostText.color = Color.white;
            CostText.text = Constants.UpgradeMaxedOut;
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
        }
        else
        {
            CostText.text = upgrade.GetCost();
        }
    }

    public virtual void Purshase()
    {
        if (CheckAvailable())
        {
            if (!upgrade.IsAffordable(playerData.Coins))
            {
                Popup.Show(Popup.popupType.message, Constants.CannotAffordIt);
                return;
            }

            playerData.RemoveCoin(upgrade.Cost);

            if (upgrade.ReachedMaxLevel)
            {
                CostText.text = Constants.UpgradeMaxedOut;
                return;
            }

            upgrade.PurchaseUpgrade();

            playerData.SetUpgrade(upgrade.GetUpgradeType, upgrade.Level);
        }
    }

    public virtual void RefreshUpgradeElement()
    {
        playerShipData = playerData.GetCurrentPlayerShipData();

        upgrade.Initialise(playerShipData);

        if (upgrade.ReachedMaxLevel)
        {
            CostText.text = Constants.UpgradeMaxedOut;
            CostText.color = Color.white;
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
            progressBar.fillAmount = 1;
            return;
        }

        progressBar.fillAmount = upgrade.ProgressPresentage;
        CostText.text = upgrade.GetCost();


        if (CheckAvailable())
        {
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
        }
        else if (!CheckAvailable())
        {
            NotAvailableImage.gameObject.SetActive(true);
            NotAvailbleText.gameObject.SetActive(true);
            NotAvailbleText.text = upgrade.GetLevelRequirment();
        }


        CostText.color = Color.red;

        if (upgrade.IsAffordable(playerData.Coins))
        {
            CostText.color = Color.white;
        }
    }

    public void OnPurschase()
    {
        RefreshUpgradeElement();
    }
}