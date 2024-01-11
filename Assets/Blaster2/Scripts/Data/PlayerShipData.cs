using System;
using UnityEngine;


[System.Serializable]
public class PlayerShipData
{
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

    public float Damage;
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
            if (Upgrades[(int)UpgradeTypeEnum.Shield] == 0)
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
            if (Upgrades[(int)UpgradeTypeEnum.ArmorUpgrade] == 1)
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

    public PlayerShipData(Player_SO player)
    {
        level = 1;
        xp = 0;
        xpToLevel = 100;

        Speed = player.baseSpeed;
        Damage = player.baseDamage;
        FireRate = player.baseFireRate;
        SuperDamage = (level * player.baseSuperDamage);
        SuperChargeTime = player.baseSpecialCountdown;
        MagnetPower = 0;
        MagnetDistance = 0;
        Upgrades = new int[Enum.GetValues(typeof(UpgradeTypeEnum)).Length];
    }
    public void NewGame(ref bool HasShieldRef)
    {
        HasShieldRef = HasShield;
    }
    
    public float GetSpeedUpgrade() { return Upgrades[(int)UpgradeTypeEnum.Speed]; }
    public float GetDamageUpgrade() { return Upgrades[(int)UpgradeTypeEnum.Damage]; }
    public float GetFireRateUpgrade() { return Upgrades[(int)UpgradeTypeEnum.FireRate]; }

    public void SetUpgrades(int[] Upgrades)
    {
        this.Upgrades = Upgrades;
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
        var DamageMultiplier = 1f * DamageUpgrade;
        var FireRateMultiplier = 0.01f * FireRateUpgrade;
        var MagnetPowerMultiplier = 3 * MagnetPowerUpgrade;
        var MagnetDistanceMultiplier = 2 * MagnetDistanceUpgrade;
        var superCooldown = 0.1f * SuperCooldownUpgrade;
        var superDamage = 1f * SuperDamageUpgrade;
        var armorUpgrade = SuperDamageUpgrade;
        //TODO PUT THE STATS IN THE RIGHT ORDER
        return new float[] {
            SpeedMultiplier,
            FireRateMultiplier,
            DamageMultiplier,
            MagnetDistanceMultiplier,
            SpeedMultiplier,
            MagnetDistanceMultiplier,
            superCooldown,
            superDamage,
            armorUpgrade
            };
    }
}
