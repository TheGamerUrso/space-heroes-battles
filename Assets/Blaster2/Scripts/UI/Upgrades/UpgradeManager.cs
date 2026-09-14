using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradesDBEntry
{
    public UpgradeData upgradeData;
    public Upgrade Upgrade;
}

public class UpgradeManager : MonoSingleton<UpgradeManager>
{
    public List<UpgradesDBEntry> ListOfUpgrades = new List<UpgradesDBEntry>();

    protected override void Init()
    {
        base.Init();
        for (int i = 0; i < ListOfUpgrades.Count; i++)
        {
            ListOfUpgrades[i].Upgrade = new Upgrade(ListOfUpgrades[i].upgradeData);
        }
    }

    public List<UpgradesDBEntry> GetUpgradeDatabase()
    {
        return ListOfUpgrades;
    }

}