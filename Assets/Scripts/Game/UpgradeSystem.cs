using UnityEngine;
using System;

[Serializable]
public class UpgradeSystem
{
    private PlayerData playerData;

    private bool ArmorUpgrade = false;
    private float GameControllerSpeedValue = 0;
    private float GameControllerDamageValue = 0;
    private float GameControllerFireRateValue = 0;
    private float GameControllerMagnetPowerValue = 0;
    private float GameControllerActivtateDistanceValue = 0;
    private float GameControllerSuperTime = 0;
    private float GameControllerSuperDamage = 0;

    [SerializeField] private float[] Upgrades;

    public void InstallArmorUpgrade()
    {
        ArmorUpgrade = true;
    }

    public bool ArmorUpgradeCheck()
    {
        if (ArmorUpgrade)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public float GetSpeedUpgrade() { return Upgrades[0]; }
    public float GetDamageUpgrade() { return Upgrades[1]; }
    public float GetFireRateUpgrade() { return Upgrades[2]; }
    public float GetMagnetDistanceUpgrade() { return Upgrades[3]; }
    public float GetMagnetPower()
    {
        return Upgrades[4];
    }
    public void SetUpgrades(float[] Upgrades)
    {
        this.Upgrades = Upgrades;
    }
    public float[] GetUpgrades()
    {
        return Upgrades;
    }
    public void SetPlayerData(PlayerData playerData)
    {
        this.playerData = playerData;
        RefreshUpgradeData();
    }

    public void ApplyUpdatesToShip(ShipStatsSystem shipStatsSystem)
    {
        shipStatsSystem.Speed += Upgrades[0];
        shipStatsSystem.Damage += Upgrades[1];
        shipStatsSystem.FireRate -= Upgrades[2];
        shipStatsSystem.MagnetPower += Upgrades[3];
        shipStatsSystem.MagnetDistance += Upgrades[4];
        shipStatsSystem.SuperChargeTime += Upgrades[5];
        shipStatsSystem.SuperDamage += Upgrades[6];    
    }

    public void RefreshUpgradeData()
    {
        PlayerData playerData = DataController.GetPlayerData();
        GameControllerSpeedValue = playerData.Upgrades[(int)UpgradeType.Speed];
        GameControllerDamageValue = playerData.Upgrades[(int)UpgradeType.Damage];
        GameControllerFireRateValue = playerData.Upgrades[(int)UpgradeType.FireRate];
        GameControllerMagnetPowerValue = playerData.Upgrades[(int)UpgradeType.MagnetStrength];
        GameControllerActivtateDistanceValue = playerData.Upgrades[(int)UpgradeType.MagnetDistance];
        GameControllerSuperTime = playerData.Upgrades[(int)UpgradeType.SuperrechargeTime];
        GameControllerSuperDamage = playerData.Upgrades[(int)UpgradeType.SuperDamage];


        if (playerData.Upgrades[(int)UpgradeType.ArmorUpgrade-1] == 1)
        {
            InstallArmorUpgrade();
        }
        else
        {
            ArmorUpgrade = false;
        }

        var SpeedMultiplier = .1f * GameControllerSpeedValue;
        var DamageMultiplier = 1f * GameControllerDamageValue;
        var FireRateMultiplier = 0.01f * GameControllerFireRateValue;
        var MagnetPowerMultiplier = 1f * GameControllerMagnetPowerValue;
        var MagnetDistanceMultiplier = 1f * GameControllerActivtateDistanceValue;
        var superTime = 0.1f * GameControllerSuperTime;
        var superDamage = 1f * GameControllerSuperDamage;

        Upgrades = new float[] {
            SpeedMultiplier,
            DamageMultiplier,
            FireRateMultiplier,
            MagnetPowerMultiplier,
            MagnetDistanceMultiplier,
            superTime,
            superDamage };
    }

    public UpgradeSystem() { }
}
