using System;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;

public class UpgradeManager : ServiceComponent<IUpgradesService>, IUpgradesService
{
    public event Action<Upgrade> OnUpgradeValueChanged;
    public event Action<string> OnUpgradeErrorOccured;

    [DictionaryDisplay]
    [SerializeField] private Dictionary<UpgradeTypeEnum, Upgrade> UpgradesDict = new Dictionary<UpgradeTypeEnum, Upgrade>();
    protected IDataService dataService;
    private PlayerData playerData;
    private PlayerShipData playerShipData;

    protected override void Awake()
    {
        base.Awake();
        dataService = GameContext.Get<IDataService>();
    }

    private void Start()
    {
        playerData = dataService.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        playerData.OnCurrentShipSelectedValueChanged += PlayerData_OnCurrentShipSelectedValueChanged;

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        playerData.OnCurrentShipSelectedValueChanged -= PlayerData_OnCurrentShipSelectedValueChanged;
    }

    private void PlayerData_OnCurrentShipSelectedValueChanged(int currentShip)
    {
        playerShipData = playerData.GetCurrentPlayerShipData();
    }

    public void BuyUpgrade(UpgradeTypeEnum upgradeType)
    {
        var upgrade = GetUpgrade(upgradeType);

        if (!upgrade.CanAffordNextLevel(playerData.playerEconomyData.Coins) ||
            !upgrade.HasRequirementMet(playerShipData.Level) || upgrade.IsMaxLevel()) return;
        {
            playerShipData.SetUpgrades(upgradeType, upgrade.Level);
            playerData.playerEconomyData.Coins -= upgrade.Cost;

            var questService = GameContext.Get<IQuestService>();
            questService.SetQuestProgressByType(QuestTypeEnum.SPEND, upgrade.Cost);

            upgrade.LevelUp();

            OnUpgradeValueChanged?.Invoke(upgrade);
        }
    }

    public Upgrade GetUpgrade(UpgradeTypeEnum upgradeType)
    {
        if (UpgradesDict.TryGetValue(upgradeType, out Upgrade value))
        {
            return value;
        }
        return null;
    }
}