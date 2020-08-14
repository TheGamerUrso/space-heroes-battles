using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreen : MonoBehaviour
{
    [SerializeField]private List<UpgradeElement> upgradeElements = new List<UpgradeElement>();
    private List<Upgrade> UpgradeDatabase;

    private void Start()
    {
        UpgradeDatabase = UpgradeManager.Instance.GetUpgradeDatabase();
        for (int i = 0; i < upgradeElements.Count; i++)
        {
            upgradeElements[i].SetUpgradeElement(UpgradeDatabase[i],this);
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
