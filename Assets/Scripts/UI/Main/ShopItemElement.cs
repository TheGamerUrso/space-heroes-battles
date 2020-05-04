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

        int upgradeTypeIndex = (int)(upgradeData.upgradeType);
        level = playerShipData.Upgrades[upgradeTypeIndex];
        cost = upgradeData.Cost;

        NammeText.text = upgradeData.upgradeType.ToString();
        CostText.text = cost.ToString();
        UpgradeIcon.sprite = upgradeData.sprite;

        if (level == 1)
        {
            CostText.color = Color.white;
            CostText.text = Constants.OutOfStock;
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
        }
    }

    public override bool CheckAvailable()
    {
        if (playerShipData.Upgrades[(int)upgradeData.upgradeType] == 0)
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
            if (playerData.Coins < cost)
            {
                Popup.Show(Popup.popupType.message, Constants.CannotAffordIt);
                return;
            }

            level = 1;
            playerData.SetUpgrade((int)(upgradeData.upgradeType), level);
            playerData.Coins -= cost;
            CostText.text = Constants.OutOfStock;
        }
        OnPurchased?.Invoke(this);
    }


    public override void RefreshUpgradeElement()
    {
        playerShipData = playerData.GetCurrentPlayerShipData();

        int upgradeIndex = (int)upgradeData.upgradeType;
        level = playerShipData.Upgrades[upgradeIndex];

        if (level == 1)
        {
            CostText.color = Color.white;
            CostText.text = Constants.OutOfStock;
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
            progressBar.fillAmount = 1;
            return;
        }

    
        cost = upgradeData.Cost;
        progressBar.fillAmount = (float)level;
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
        }

        CostText.color = Color.white;

        if (cost > playerData.Coins)
        {
            CostText.color = Color.red;
        }

        if (level == 1)
        {
        
        }
    }
}
