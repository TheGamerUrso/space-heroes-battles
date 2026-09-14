using System;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeElement : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private Image progressbar;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Price;
    [SerializeField] private TextMeshProUGUI MessageText;
    [SerializeField] private Image Icon;

    [SerializeField] private GameObject NotAvailable;
    private UpgradeManager upgradeManager;

    private Color TextdefaultColor;
    private PlayerData playerData;
    private PlayerShipData playerShipData;
    [SerializeField] private UpgradeData upgradeData;
    private Upgrade upgrade;

    public void SetUpgradeElement(UpgradeManager upgradeManager,PlayerData playerData)
    {
        this.upgradeManager = upgradeManager;
        this.playerData = playerData;
        this.playerShipData = playerData.GetCurrentPlayerShipData();
        upgrade = upgradeManager.GetUpgrade(upgradeData.upgradeType);
        upgradeManager.OnUpgradeValueChanged += Upgrade_OnUpdateValueChanged;
        upgradeManager.OnUpgradeErrorOccured += UpgradeManager_OnUpgradeErrorOccured;
        Name.text = upgradeManager.GetUpgradeName(upgrade.upgradeData.upgradeType);
        Icon.sprite = upgrade.upgradeData.sprite;
        TextdefaultColor = Price.color;

        Refresh();
    }

    private void UpgradeManager_OnUpgradeErrorOccured(string message)
    {
        Warn(message); 
    }

    private void Upgrade_OnUpdateValueChanged(Upgrade obj)
    {
        Refresh();
    }

    private void Update()
    {
        Price.color = playerData.Coins < upgrade.Cost ? Color.red : Color.green;
        buyButton.interactable = playerData.Coins < upgrade.Cost ? false : true;
    }

    public void Refresh()
    {
     
        Price.text = upgradeManager.GetCost(upgrade.upgradeData.upgradeType);


        progressbar.fillAmount = upgrade.ProgressPresentage;

        MessageText.gameObject.SetActive(false);
        NotAvailable.SetActive(false);
        Price.color = TextdefaultColor;
        buyButton.interactable = true;

        if (upgrade.ProgressPresentage == 1)
        {
            buyButton.interactable = false;
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
            buyButton.interactable = false;
        }

        if (playerShipData.level < upgradeManager.GetLevelRequirment(upgrade.upgradeData.upgradeType))
        {
            NotAvailable.SetActive(true);
            Warn(Constants.UnlockedAtLvl + upgradeManager.GetLevelRequirment(upgrade.upgradeData.upgradeType));
            buyButton.interactable = false;
        }
    }


    public void BuyButton()
    {
        if (upgrade.ProgressPresentage == 1)
        {
            buyButton.interactable = false;
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

        if (playerData.Coins >= upgrade.Cost && playerShipData.level >= upgradeManager.GetLevelRequirment(upgrade.upgradeData.upgradeType))
        {
            if (upgrade.ProgressPresentage == 1)
            {
                return;
            }

            if (playerData.Coins >= upgrade.Cost && playerShipData.level >= upgradeManager.GetLevelRequirment(upgrade.upgradeData.upgradeType))
            {
                upgrade.LevelUp();
            }
        }


        upgradeManager.BuyUpgrade(upgradeData.upgradeType);
    }

    public void Warn(string message)
    {
        MessageText.text = message;
        MessageText.gameObject.SetActive(true);
    }
}
