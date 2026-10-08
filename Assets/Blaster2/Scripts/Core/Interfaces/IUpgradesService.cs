using System;
using UnityEngine;

public interface IUpgradesService
{
    public event Action<Upgrade> OnUpgradeValueChanged;
    public event Action<string> OnUpgradeErrorOccured;
    public void BuyUpgrade(UpgradeTypeEnum upgradeType);
}
