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

    public string GetUpgradeName()
    {
        return $"{upgradeData.upgradeType.ToString()}";
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
        Cost = GetCost();
    }

    public int GetCost()
    {
        if (upgradeData.CostPerLevel.Length > 0)
        {
            return upgradeData.CostPerLevel[Level];
        }
        else
        {
            return upgradeData.Cost;
        }
    }

    public string GetLevelRequirmentToString(UpgradeTypeEnum upgradeTypeEnum, int Level = 1)
    {
        return  $"{Constants.UnlockedAtLvl} {upgradeData.LevelRequirementPerLevel[Level].ToString()}" +
            $" Required";
    }

    public string GetUpgradeName(UpgradeTypeEnum upgradeTypeEnum)
    {
        return $"{upgradeTypeEnum.ToString()}";
    }

    public int GetLevelRequirment(UpgradeTypeEnum upgradeType)
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
    public bool HasRequirementMet(int currentLevel)
    {
        return  currentLevel >= GetLevelRequirment(upgradeData.upgradeType);
    }

    public bool IsMaxLevel()
    {
        // If current level is equal to or greater than max level (e.g., 10)
        return Level >= upgradeData.MaxLevel;
    }

    public bool CanAffordNextLevel(int playerCurrency)
    {
        int levelRequirement = GetLevelRequirment(upgradeData.upgradeType);
        if (IsMaxLevel())
        {
            return false;
        }
   
        int nextLevelCost = GetCost();

        return playerCurrency >= nextLevelCost;
    }
}
