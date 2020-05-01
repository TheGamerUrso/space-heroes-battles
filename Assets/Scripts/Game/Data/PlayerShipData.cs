using System;
using UnityEngine;


[System.Serializable]
public class PlayerShipData
{
    public int MaxLevel = 20;

    public int level;
    public float xp;
    public float xpToLevel;
    public float XPPresentage
    {
        get
        {
            return (float)xp / xpToLevel;
        }
    }

    #region Player Upgrades
    public int[] Upgrades;

    public float FireRate;
    public float Speed;


    public float SuperDamage;
    public float LaserDamage;
    public float SuperChargeTime;
    public float MagnetPower;
    public float MagnetDistance;

    public bool HasShield
    {
        get
        {
            if (Upgrades[(int)UpgradeType.Shield] == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public bool HasArmorUpgrade
    {
        get
        {
            if (Upgrades[(int)UpgradeType.ArmorUpgrade] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion

    public PlayerShipData()
    {
        MaxLevel = 20;
        level = 1;
        xp = 0;
        xpToLevel = 100;
        Upgrades = new int[Enum.GetValues(typeof(UpgradeType)).Length];
    }

    public float GetSpeedUpgrade() { return Upgrades[(int)UpgradeType.Speed]; }
    public float GetDamageUpgrade() { return Upgrades[(int)UpgradeType.Damage]; }
    public float GetFireRateUpgrade() { return Upgrades[(int)UpgradeType.FireRate]; }

    public void SetUpgrades(int[] Upgrades)
    {
        this.Upgrades = Upgrades;
    }
    
    public int[] GetUpgrades()
    {
        return Upgrades;
    }

    public float[] GetCalculatedUpgradeStats()
    {
        var GameControllerSpeedValue = Upgrades[(int)UpgradeType.Speed];
        var GameControllerDamageValue = Upgrades[(int)UpgradeType.Damage];
        var GameControllerFireRateValue = Upgrades[(int)UpgradeType.FireRate];
        var GameControllerMagnetPowerValue = Upgrades[(int)UpgradeType.MagnetStrength];
        var GameControllerActivtateDistanceValue = Upgrades[(int)UpgradeType.MagnetDistance];
        var GameControllerSuperTime = Upgrades[(int)UpgradeType.SuperrechargeTime];
        var GameControllerSuperDamage = Upgrades[(int)UpgradeType.SuperDamage];

        var SpeedMultiplier = .1f * GameControllerSpeedValue;
        var DamageMultiplier = 1f * GameControllerDamageValue;
        var FireRateMultiplier = 0.01f * GameControllerFireRateValue;
        var MagnetPowerMultiplier = 3 * GameControllerMagnetPowerValue;
        var MagnetDistanceMultiplier = 2 * GameControllerActivtateDistanceValue;
        var superCooldown = 0.1f * GameControllerSuperTime;
        var superDamage = 1f * GameControllerSuperDamage;

        //TODO PUT THE STATS IN THE RIGHT ORDER
        return new float[] {
            SpeedMultiplier,
            FireRateMultiplier,
            DamageMultiplier,
            superDamage,
            superCooldown,
            MagnetPowerMultiplier,
            MagnetDistanceMultiplier};
    }
}
