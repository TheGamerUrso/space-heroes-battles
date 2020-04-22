using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemElement : UpgradeElement
{
    public override void InitUpgradeElement(UpgradeManager upgradeManager)
    {
        this.upgradeManager = upgradeManager;
        upgradeManager.SubscribePurchasable(this);

        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        level = playerShipData.Upgrades[(int)(upgradeData.upgradeType) - 1];
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
        if (playerShipData.Upgrades[(int)(upgradeData.upgradeType) - 1] == 0)
        {
            return true;
        }
        else
        {
            Popup.Show(Popup.popupType.message, "Max Lvl Reached");
        }
        return false;
    }

    public override void Upgrade()
    {
        level = 1;
        CostText.text = Constants.OutOfStock;
    }


    public override void RefreshUpgradeElement()
    {
        playerShipData = playerData.GetCurrentPlayerShipData();

        level = playerShipData.Upgrades[((int)(upgradeData.upgradeType) - 1)];

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
