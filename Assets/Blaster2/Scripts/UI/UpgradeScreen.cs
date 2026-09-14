using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpgradeScreen : UIView
{
    [SerializeField] private List<UpgradeElement> upgradeElements = new List<UpgradeElement>();
    private List<UpgradesDBEntry> UpgradeDatabase;
    protected UpgradeManager upgradeManager;

    private void Start()
    {
        UpgradeDatabase = upgradeManager.GetUpgradeDatabase();
        for (int i = 0; i < upgradeElements.Count; i++)
        {
            upgradeElements[i].SetUpgradeElement(UpgradeDatabase[i].Upgrade,this);
        }
  
    }

    public void RefreshUpgrades()
    {
        for (int i = 0; i < upgradeElements.Count; i++)
        {
            upgradeElements[i].Refresh();
        }
    }
        
}
