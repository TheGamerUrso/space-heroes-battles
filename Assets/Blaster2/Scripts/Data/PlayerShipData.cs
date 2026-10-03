using System;
using UnityEngine;


[System.Serializable]
public class PlayerShipData : ShipData
{
    public Action<int, float, float> OnXPValueChanged;
    public float xp;
    public float xpToLevel;
    public float XPPresentage
    {
        get
        {
            return (float)xp / xpToLevel;
        }
    }

    public int[] Upgrades;
    public float SuperDamage;
    public float LaserDamage;
    public float SuperChargeTime;
    public float MagnetPower;
    public float MagnetDistance;

    public float ChargePower = 0;
    public int PowerPackCollected = 0;
    public bool HasArmorUpgrade { get; set; }

    public PlayerShipData(Player_SO player)
    {
        Level = 1;
        xp = 0;
        xpToLevel = 100;
        Health = player.baseHealth;
        Speed = player.baseSpeed;
        FireRate = player.baseFireRate;
        Damage = player.baseDamage;
        SuperDamage = (Level * player.baseSuperDamage);
        SuperChargeTime = player.baseSpecialCountdown;
        MagnetPower = 0;
        MagnetDistance = 0;
        Upgrades = new int[Enum.GetValues(typeof(UpgradeTypeEnum)).Length];
    }


    public void EarnXP(float ammount)
    {
        if (Level < 20)
        {
            xp += ammount;

            if (xp >= xpToLevel)
            {
                Level++;
                xp = 0;

                xpToLevel = (Level / 10 + Level % 10) * 100 * Mathf.Pow(10, Level / 10);
            }
        }
        else
        {
            xp = 0;
            xpToLevel = 0;
        }
        OnXPValueChanged?.Invoke(Level, xp, xpToLevel);
    }
    public float GetSpeedUpgrade() { return Upgrades[(int)UpgradeTypeEnum.Speed]; }
    public float GetDamageUpgrade() { return Upgrades[(int)UpgradeTypeEnum.Damage]; }
    public float GetFireRateUpgrade() { return Upgrades[(int)UpgradeTypeEnum.FireRate]; }

    public void SetUpgrades(int upgrade, int value)
    {
        Upgrades[upgrade] = value;
        SaveSystem.SaveGame();
    }

    public int[] GetUpgrades()
    {
        return Upgrades;
    }

    public void SetUpgradeByType(UpgradeTypeEnum upgradeType, int Level)
    {
        Upgrades[(int)upgradeType] = Level;
    }

    public float[] GetCalculatedUpgradeStats()
    {
        var SpeedUpgrade = Upgrades[(int)UpgradeTypeEnum.Speed];
        var DamageUpgrade = Upgrades[(int)UpgradeTypeEnum.Damage];
        var FireRateUpgrade = Upgrades[(int)UpgradeTypeEnum.FireRate];
        var MagnetPowerUpgrade = Upgrades[(int)UpgradeTypeEnum.MagnetStrength];
        var MagnetDistanceUpgrade = Upgrades[(int)UpgradeTypeEnum.MagnetDistance];
        var SuperCooldownUpgrade = Upgrades[(int)UpgradeTypeEnum.SuperrechargeTime];
        var SuperDamageUpgrade = Upgrades[(int)UpgradeTypeEnum.SuperDamage];
        var HasArmorUpgrade = Upgrades[(int)UpgradeTypeEnum.ArmorUpgrade];

        var SpeedMultiplier = 0.25f * SpeedUpgrade;
        var DamageMultiplier = .1f * DamageUpgrade;
        var FireRateMultiplier = 0.01f * FireRateUpgrade;
        var MagnetPowerMultiplier = 3 * MagnetPowerUpgrade;
        var MagnetDistanceMultiplier = 2 * MagnetDistanceUpgrade;
        var superCooldown = 0.1f * SuperCooldownUpgrade;
        var superDamage = 1f * SuperDamageUpgrade;
        var armorUpgrade = SuperDamageUpgrade;

        return new float[] {
            SpeedMultiplier,
            FireRateMultiplier,
            DamageMultiplier,
            MagnetPowerMultiplier,
            SpeedMultiplier,
            MagnetDistanceMultiplier,
            superCooldown,
            superDamage,
            armorUpgrade
            };
    }
}
