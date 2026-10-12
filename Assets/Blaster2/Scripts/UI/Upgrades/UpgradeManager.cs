using System;
using System.Collections.Generic;
using TheGamerUrso.Core;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class UpgradeManager : ServiceComponent<IUpgradesService>, IUpgradesService
{
    public event Action<Upgrade> OnUpgradeValueChanged;
    public event Action<string> OnUpgradeErrorOccured;

    [DictionaryDisplay]
    [SerializeField] private Dictionary<UpgradeTypeEnum, Upgrade> UpgradesDict = new Dictionary<UpgradeTypeEnum, Upgrade>();
    protected IDataService dataService;
    private INotificationService notificationService;

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

        notificationService = GameContext.Get<INotificationService>();
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

        bool ErrorOccured = false;

        if (upgrade.IsMaxLevel())
        {
            notificationService.Show(new ErrorDialogEvent() { Type = popupType.error, Message = "Upgrade Maxed Out" });
            ErrorOccured = true;
        }

        if (!upgrade.CanAffordNextLevel(playerData.playerEconomyData.Coins))
        {
            notificationService.Show(new ErrorDialogEvent() { Type = popupType.error, Message = "Insufficient funds" });
            ErrorOccured = true;
        }

        if (!upgrade.HasRequirementMet(playerShipData.Level))
        {
            notificationService.Show(new ErrorDialogEvent() { Type = popupType.error, Message = $"Requires Lvl {upgrade.GetLevelRequirment(upgrade.upgradeData.upgradeType)}" });
            ErrorOccured = true;
        }


        if (ErrorOccured) return;

        playerShipData.SetUpgrades(upgradeType, upgrade.Level);
        playerData.playerEconomyData.Coins -= upgrade.Cost;

        var questService = GameContext.Get<IQuestService>();
        questService.SetQuestProgressByType(QuestTypeEnum.SPEND, upgrade.Cost);

        upgrade.LevelUp();

        OnUpgradeValueChanged?.Invoke(upgrade);

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