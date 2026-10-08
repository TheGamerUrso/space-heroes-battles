using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class PlayerShipData : ShipData
{
    public event Action<float> OnPowerPackCollected;
    public event Action<float> OnSuperChargedValueChanged;
    public Action<int, float, float> OnXPValueChanged;
    public Action<int> OnLevelUp;

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

    [SerializeField]
    private Dictionary<UpgradeTypeEnum, int> UpgradeDict = new Dictionary<UpgradeTypeEnum, int>();

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

        var names = Enum.GetValues(typeof(UpgradeTypeEnum)).Cast<UpgradeTypeEnum>().ToList();

        foreach (var item in names)
        {
            UpgradeDict.Add(item, 0);
        }
    }


    public void EarnXP(float ammount)
    {
        if (Level < 20)
        {
            xp += ammount;

            if (xp >= xpToLevel)
            {
                Level++;
                OnLevelUp?.Invoke(Level);
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

    public float GetUpgrade(UpgradeTypeEnum upgradeType)
    {
        if(UpgradeDict.TryGetValue(upgradeType,out var upgrade))
        {
            var HealthUpgrade = Upgrades[(int)UpgradeTypeEnum.Health];
            return upgrade;
        }
        return 0;
    }

    public void SetUpgrades(UpgradeTypeEnum type, int value)
    {
        if (UpgradeDict.TryGetValue(type, out var upgrade))
        {
            UpgradeDict[type] = value;
        }
    }

    public ReadOnlyCollection<KeyValuePair<UpgradeTypeEnum, int>> GetUpgrades()
    {
        ReadOnlyCollection<KeyValuePair<UpgradeTypeEnum,int>> readOnlyDinosaurs =
            new ReadOnlyCollection<KeyValuePair<UpgradeTypeEnum, int>>(UpgradeDict.ToList());
        return readOnlyDinosaurs;
    }

    public float[] GetCalculatedUpgradeStats()
    {
        var HealthUpgrade = Upgrades[(int)UpgradeTypeEnum.Health];
        var SpeedUpgrade = Upgrades[(int)UpgradeTypeEnum.Speed];
        var DamageUpgrade = Upgrades[(int)UpgradeTypeEnum.Damage];
        var FireRateUpgrade = Upgrades[(int)UpgradeTypeEnum.FireRate];
        var MagnetPowerUpgrade = Upgrades[(int)UpgradeTypeEnum.MagnetStrength];
        var MagnetDistanceUpgrade = Upgrades[(int)UpgradeTypeEnum.MagnetDistance];
        var SuperCooldownUpgrade = Upgrades[(int)UpgradeTypeEnum.SuperrechargeTime];
        var SuperDamageUpgrade = Upgrades[(int)UpgradeTypeEnum.SuperDamage];
        var HasArmorUpgrade = Upgrades[(int)UpgradeTypeEnum.ArmorUpgrade];

        var healthMultiplier = 5 * HealthUpgrade;
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

    public void UpdateSuperCharge(float value)
    {
        ChargePower += value;
        OnSuperChargedValueChanged?.Invoke(ChargePower);
    }
    public void SetSuperCharge(float value)
    {
        ChargePower = value;
        OnSuperChargedValueChanged?.Invoke(ChargePower);
    }

    public void UpdatePowerPackCollected(int value)
    {
        PowerPackCollected += value;
        OnPowerPackCollected?.Invoke(PowerPackCollected);
        if (PowerPackCollected >= 5)
        {
            PowerPackCollected = 0;
        }
    }


}
