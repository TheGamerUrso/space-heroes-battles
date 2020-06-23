using System;
using UnityEngine;

[Serializable]
public class Upgrade
{
    public UpgradeData upgradeData;

    protected int level;
    protected int requirement;
    protected int cost;

    #region Properties
    public int GetUpgradeType
    {
        get
        {
            return (int)upgradeData.upgradeType;
        }
    }

    public Sprite UpgradeIcon
    {
        get
        {
            return upgradeData.sprite;
        }
    }

    public int Level
    {
        get { return level; }
    }

    public int Cost
    {
        get { return cost; }
    }

    public float ProgressPresentage
    {
        get
        {
            return (float)level / 10;
        }
    }

    public bool ReachedMaxLevel
    {
        get
        {
            if (upgradeData.CostPerLevel.Length == 0)
            {
                if (level == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (level >= upgradeData.MaxLevel || upgradeData.CostPerLevel.Length - 1 <= level)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    #endregion

    public void Initialise(PlayerShipData playerShipData)
    {
        int upgradeTypeIndex = (int)(upgradeData.upgradeType);
        level = playerShipData.Upgrades[upgradeTypeIndex];
        if (upgradeData.CostPerLevel.Length > 0)
        {
            cost = upgradeData.CostPerLevel[level];
        }
        else
        {
            cost = upgradeData.Cost;
        }
    }

    public bool CheckRequirement(int playerLevel)
    {
        if (playerLevel >= upgradeData.LevelRequirementPerLevel[level])
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void PurchaseUpgrade()
    {
        level++;
    }


    public bool IsAffordable(int PlayerCoins)
    {
        if (PlayerCoins >= cost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public string GetCost()
    {
        return $"{cost.ToString()}";
    }

    public string GetLevelRequirment()
    {
        return $"{Constants.UnlockedAtLvl} {upgradeData.LevelRequirementPerLevel[level].ToString()} Required";
    }

    public string GetUpgradeName()
    {
        return $"{upgradeData.upgradeType.ToString()}";
    }
}
