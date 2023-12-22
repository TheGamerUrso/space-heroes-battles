using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[Serializable]
public class Upgrade
{
    private PlayerData playerData;
    private PlayerShipData playerShipData;

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

    ~Upgrade()
    {
        Events.OnShipSelectValueChanged -= (x) => { SetPlayerShipData(); };
    }

    public Upgrade(UpgradeData upgradeData)
    {
        this.upgradeData = upgradeData;

        Level = 0;

        Events.OnShipSelectValueChanged += (x) => { SetPlayerShipData(); };

        playerData = PersistantData.GetPlayerData();
        SetPlayerShipData();


        if (upgradeData.CostPerLevel.Length > 0)
        {
            Cost = upgradeData.CostPerLevel[Level];
        }
        else
        {
            Cost = upgradeData.Cost;
        }
    }

    public int GetLevelRequirment()
    {
        if (upgradeData.LevelRequirementPerLevel.Length > 0)
        {
            return upgradeData.LevelRequirementPerLevel[Level];
        }
        else
        {
            return 1;
        }
    }

    public void Buy()
    {
        Level++;

        playerShipData.SetUpgradeByType(upgradeData.upgradeType, Level);

        playerData.AbstractCoins(Cost);

        if (upgradeData.CostPerLevel.Length > 0)
        {
            Cost = upgradeData.CostPerLevel[Level];
        }
        else
        {
            Cost = upgradeData.Cost;
        }
    }

    public void SetPlayerShipData()
    {
        playerShipData = playerData.GetCurrentPlayerShipData();
        Level = playerShipData.Upgrades[(int)upgradeData.upgradeType];
    }

    public string GetCost()
    {
        return $"{Cost.ToString()}";
    }

    public string GetLevelRequirmentToString()
    {
        return $"{Constants.UnlockedAtLvl} {upgradeData.LevelRequirementPerLevel[Level].ToString()} Required";
    }

    public string GetUpgradeName()
    {
        return $"{upgradeData.upgradeType.ToString()}";
    }
}
