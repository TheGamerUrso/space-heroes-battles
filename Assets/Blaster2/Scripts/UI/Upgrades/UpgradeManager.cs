using System;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public event Action<Upgrade> OnUpgradeValueChanged;
    public event Action<string> OnUpgradeErrorOccured;

    [DictionaryDisplay]
    [SerializeField] private Dictionary<UpgradeTypeEnum, Upgrade> UpgradesDict = new Dictionary<UpgradeTypeEnum, Upgrade>();
    protected IDataService dataService;
    private PlayerData playerData;
    private PlayerShipData playerShipData;
    protected void Awake()
    {
        dataService = GameContext.Get<IDataService>();
    }

    private void Start()
    {
        playerData = dataService.GetPlayerData();
    }

    private void OnShipSelected(int shipSelected)
    {
        foreach(Upgrade upgrade in UpgradesDict.Values)
        {
            upgrade.SetUpgrade(playerShipData.Upgrades[(int)upgrade.upgradeData.upgradeType]);
        }
    }

    public Upgrade GetUpgrade(UpgradeTypeEnum upgradeType)
    {
        if(UpgradesDict.TryGetValue(upgradeType, out Upgrade value))
        {
            return value;
        }
        return null;
    }

    public void BuyUpgrade(UpgradeTypeEnum upgradeType)
    {
        var upgrade = GetUpgrade(upgradeType);
        if (upgrade.ProgressPresentage == 1)
        {
            return;
        }
        var upgradeFound = GetUpgrade(upgradeType);

        playerShipData.SetUpgradeByType(upgradeType, upgradeFound.Level);

        playerData.AbstractCoins(upgradeFound.Cost);

        var questService = GameContext.Get<IQuestService>();
        questService.SetQuestProgressByType(QuestTypeEnum.SPEND, upgradeFound.Cost);

        if (upgrade.ProgressPresentage == 1)
        {
            if (upgrade.upgradeData.MaxLevel > 1)
            {
                OnUpgradeErrorOccured?.Invoke(Constants.UpgradeMaxedOut);
            }
            else if (upgrade.upgradeData.MaxLevel == 1)
            {
                OnUpgradeErrorOccured?.Invoke(Constants.OutOfStock);
            }
            return;
        }


        if (playerData.Coins >= upgrade.Cost && playerShipData.level >= GetLevelRequirment(upgrade.upgradeData.upgradeType))
        {
            if (upgrade.ProgressPresentage == 1)
            {
                return;
            }

            upgrade.LevelUp();

            OnUpgradeValueChanged?.Invoke(upgrade);
        }
    }

    public string GetCost(UpgradeTypeEnum upgradeType)
    {
        var upgradeFound = GetUpgrade(upgradeType);
        return $"{(upgradeFound!=null ? upgradeFound.Cost.ToString() : 0)}";
    }

    public string GetLevelRequirmentToString(UpgradeTypeEnum upgradeTypeEnum,int Level = 1)
    {
        var upgradeFound = GetUpgrade(upgradeTypeEnum);
        return $"{(upgradeFound != null ? $"{Constants.UnlockedAtLvl} {upgradeFound.upgradeData.LevelRequirementPerLevel[Level].ToString()} Required" : 0)}";
    }

    public string GetUpgradeName(UpgradeTypeEnum upgradeTypeEnum)
    {
        return $"{upgradeTypeEnum.ToString()}";
    }


    public int GetLevelRequirment(UpgradeTypeEnum upgradeType)
    {
        var upgradeFound = GetUpgrade(upgradeType);
        if (upgradeFound.upgradeData.LevelRequirementPerLevel.Length > 0)
        {
            return upgradeFound.upgradeData.LevelRequirementPerLevel[upgradeFound.Level];
        }
        else
        {
            return 1;
        }
    }

}