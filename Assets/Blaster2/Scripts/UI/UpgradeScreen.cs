using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.UIElements;



public class UpgradeScreen : UIView
{
    [SerializeField] private List<UpgradeElement> upgradeElements = new List<UpgradeElement>();
    [SerializeField] private UpgradeManager upgradeManager;

    private IDataService dataService;

    private void Start()
    {
        dataService = GameContext.Get<IDataService>();

        upgradeManager.OnUpgradeValueChanged += UpgradeManager_OnUpgradeValueChanged;
        for (int i = 0; i < upgradeElements.Count; i++)
        {
            upgradeElements[i].SetUpgradeElement(upgradeManager, dataService.GetPlayerData());
        }
    }

    private void OnDestroy()
    {
        upgradeManager.OnUpgradeValueChanged -= UpgradeManager_OnUpgradeValueChanged;
    }

    private void UpgradeManager_OnUpgradeValueChanged(Upgrade upgrade)
    {
        RefreshUpgrades();
    }

    public void RefreshUpgrades()
    {
        for (int i = 0; i < upgradeElements.Count; i++)
        {
            upgradeElements[i].Refresh();
        }
    }

    public void BuyUpgrade(UpgradeTypeEnum upgradeType)
    {      
       
    }

}
