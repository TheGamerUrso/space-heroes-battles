using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemElement : UpgradeElement
{
    public override void InitUpgradeElement(UpgradeManager upgradeManager)
    {
        this.upgradeManager = upgradeManager;
        upgradeManager.SubscribePurchasable(this);

        playerShipData = playerData.GetCurrentPlayerShipData();

        upgrade.Initialise(playerShipData);

        NammeText.text = upgrade.GetUpgradeName();

        if (upgrade.ReachedMaxLevel)
        {
            CostText.color = Color.white;
            CostText.text = Constants.OutOfStock;
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
        }
    }

    public override bool CheckAvailable()
    {
        if (playerShipData.Upgrades[upgrade.GetUpgradeType] == 0)
        {
            return true;
        }
        else
        {
            Popup.Show(Popup.popupType.message, "Owned Already");
        }
        return false;
    }

    public override void Purshase()
    {
        if (CheckAvailable())
        {
            if (upgrade.IsAffordable(playerData.coins))
            {
                Popup.Show(Popup.popupType.message, Constants.CannotAffordIt);
                return;
            }

            playerData.Coins -= upgrade.Cost;

            if (upgrade.ReachedMaxLevel)
            {
                CostText.text = Constants.UpgradeMaxedOut;
                return;
            }

            upgrade.PurchaseUpgrade();

            playerData.SetUpgrade(upgrade.GetUpgradeType, upgrade.Level);
        }
        Events.OnPurchased?.Invoke(this);
    }


    public override void RefreshUpgradeElement()
    {
        playerShipData = playerData.GetCurrentPlayerShipData();
        upgrade.Initialise(playerShipData);

        if (upgrade.ReachedMaxLevel)
        {
            CostText.color = Color.white;
            CostText.text = Constants.OutOfStock;
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
        }

        CostText.color = Color.red;

        if (upgrade.IsAffordable(playerData.coins))
        {
            CostText.color = Color.white;
        }
    }
}
