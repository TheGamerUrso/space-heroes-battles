using System;
using TheGamerUrso.Core;
using UnityEngine;


[Serializable]
public class Upgrade
{
    public UpgradeData upgradeData;
    public int Level; 
    public int Cost;

    public float ProgressPresentage
    {
        get
        {
            return (float)Level / (float)upgradeData.MaxLevel;
        }
    }
    public void LevelUp()
    {
        Level++;

        if (upgradeData.CostPerLevel.Length > 0)
        {
            Cost = upgradeData.CostPerLevel[Level];
        }
        else
        {
            Cost = upgradeData.Cost;
        }
    }

    public void SetUpgrade(int level)
    {
        Level = level;
        if (upgradeData.CostPerLevel.Length > 0)
        {
            Cost = upgradeData.CostPerLevel[Level];
        }
        else
        {
            Cost = upgradeData.Cost;
        }
    }
 
}
