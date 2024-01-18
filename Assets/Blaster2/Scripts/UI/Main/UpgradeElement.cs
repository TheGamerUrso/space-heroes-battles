using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Doozy.Engine.UI;

public class UpgradeElement : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerShipData playerShipData;

    [SerializeField] private UIButton buyButton;
    [SerializeField] private Image progressbar;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Price;
    [SerializeField] private TextMeshProUGUI MessageText;
    [SerializeField] private Image Icon;

    [SerializeField] private GameObject NotAvailable;


    private Upgrade upgrade;
    private UpgradeScreen upgradeScreen;

    private Color TextdefaultColor;

    private void OnEnable()
    {
        Events.RefreshUpdateData?.Invoke();
    }

    private void OnDestroy()
    {
        Events.RefreshUpdateData -= Refresh;
    }

    private void Start()
    {
        Events.RefreshUpdateData += Refresh;
    }

    public void SetUpgradeElement(Upgrade upgrade, UpgradeScreen upgradeScreen)
    {
        this.upgrade = upgrade;
        Name.text = upgrade.GetUpgradeName();
        Price.text = upgrade.GetCost();
        Icon.sprite = upgrade.upgradeData.sprite;
        this.upgradeScreen = upgradeScreen;
        playerData = PersistantData.GetPlayerData();

        TextdefaultColor = Price.color;

        Refresh();
    }

    public void Refresh()
    {
        if (playerData == null)
        {
            return;
        }

        playerShipData = playerData.GetCurrentPlayerShipData();
        progressbar.fillAmount = upgrade.ProgressPresentage;

        MessageText.gameObject.SetActive(false);
        NotAvailable.SetActive(false);
        Price.color = TextdefaultColor;
        buyButton.EnableButton();

        if (upgrade.ProgressPresentage == 1)
        {
            buyButton.DisableButton();
            if (upgrade.upgradeData.MaxLevel > 1)
            {
                Warn(Constants.UpgradeMaxedOut);
            }
            else if (upgrade.upgradeData.MaxLevel == 1)
            {
                Warn(Constants.OutOfStock);
            }
            return;
        }


        if (playerData.Coins < upgrade.Cost)
        {
            Price.color = Color.red;
            buyButton.DisableButton();
        }

        if (playerShipData.level < upgrade.GetLevelRequirment())
        {
            NotAvailable.SetActive(true);
            Warn(Constants.UnlockedAtLvl + upgrade.GetLevelRequirment());
            buyButton.DisableButton();
        }
    }


    public void BuyButton()
    {
        if (playerData.Coins >= upgrade.Cost && playerShipData.level >= upgrade.GetLevelRequirment())
        {
            if (upgrade.ProgressPresentage == 1)
            {
                return;
            }

            if (playerData.Coins >= upgrade.Cost && playerShipData.level >= upgrade.GetLevelRequirment())
            {
                upgrade.Buy();
            }

            upgradeScreen.RefreshUpgrades();
        }
    }

    public void Warn(string message)
    {
        MessageText.text = message;
        MessageText.gameObject.SetActive(true);
    }
}
