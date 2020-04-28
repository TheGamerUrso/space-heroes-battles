using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeElement : MonoBehaviour, IPurchasable
{
    public delegate void Purchased(UpgradeElement upgradeElement);
    public Purchased OnPurchased;

    public UpgradeData upgradeData;

    [SerializeField] protected TextMeshProUGUI CostText;
    [SerializeField] protected TextMeshProUGUI NammeText;
    [SerializeField] protected TextMeshProUGUI NotAvailbleText;

    [SerializeField] protected Image progressBar;
    [SerializeField] protected Image UpgradeIcon;
    [SerializeField] protected Image NotAvailableImage;

    protected int level;

    protected int requirement;
    protected int cost;

    public int Level
    {
        get { return level; }
    }

    public int Cost
    {
        get { return cost; }
    }


    protected PlayerData playerData;
    protected PlayerShipData playerShipData;
    protected UpgradeManager upgradeManager;
    private bool maxOut;

    private void OnEnable()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        RefreshUpgradeElement();
    }

    public virtual bool CheckAvailable()
    {
        if (upgradeData.CostPerLevel.Length - 1 <= level)
        {
            Popup.Show(Popup.popupType.message, Constants.UpgradeMaxedOut);
            return false;
        }

        if (playerShipData.level >= upgradeData.LevelRequirementPerLevel[level])
        {
            return true;
        }
        else
        {
            Popup.Show(Popup.popupType.message, "Lv " + upgradeData.LevelRequirementPerLevel[level].ToString() + " Required");
        }

        return false;
    }

    private void Start()
    {
        InitUpgradeElement(UpgradeManager.Instance);
    }

    public virtual void InitUpgradeElement(UpgradeManager upgradeManager)
    {
        this.upgradeManager = upgradeManager;
        upgradeManager.SubscribePurchasable(this);

        playerShipData = playerData.GetCurrentPlayerShipData();


        level = playerShipData.Upgrades[(int)(upgradeData.upgradeType)];
        cost = upgradeData.CostPerLevel[level];

        NammeText.text = upgradeData.upgradeType.ToString();


        UpgradeIcon.sprite = upgradeData.sprite;

        if (level >= upgradeData.MaxLevel)
        {
            CostText.color = Color.white;
            CostText.text = Constants.UpgradeMaxedOut;
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
            maxOut = true;
        }
        else
        {
            CostText.text = cost.ToString();
        }
    }

    public virtual void Upgrade()
    {
        if (upgradeData.CostPerLevel.Length - 1 <= level)
        {
            CostText.text = Constants.UpgradeMaxedOut;
            return;
        }

        level++;
    }

    public virtual void Purshase()
    {
        if (CheckAvailable())
        {
            if (playerData.Coins < cost)
            {
                Popup.Show(Popup.popupType.message, Constants.CannotAffordIt);
                return;
            }

            playerData.Coins -= cost;

            Upgrade();

            OnPurchased?.Invoke(this);
        }
    }

    public virtual void RefreshUpgradeElement()
    {
        playerShipData = playerData.GetCurrentPlayerShipData();

        level = playerShipData.Upgrades[(int)(upgradeData.upgradeType)];


        if (level >= upgradeData.MaxLevel)
        {
            CostText.text = Constants.UpgradeMaxedOut;
            CostText.color = Color.white;
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
            progressBar.fillAmount = 1;
            return;
        }

        cost = upgradeData.CostPerLevel[level];
        progressBar.fillAmount = (float)level / 10;
        CostText.text = cost.ToString();


        if (CheckAvailable())
        {
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
        }
        else if (!CheckAvailable())
        {
            NotAvailableImage.gameObject.SetActive(true);
            NotAvailbleText.gameObject.SetActive(true);
            NotAvailbleText.text = Constants.UnlockedAtLvl + upgradeData.LevelRequirementPerLevel[level];
        }

        CostText.color = Color.white;
        if (cost > playerData.Coins)
        {
            CostText.color = Color.red;
        }
    }

    public void OnPurschase()
    {
        RefreshUpgradeElement();
    }
}