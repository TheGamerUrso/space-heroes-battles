using UnityEngine;

[System.Serializable]
public struct ShipStats
{
    public int id;
    public int Level;
    public float xpToLevel;
    public float xp;
    public float currentXp;

    [Header("Attributes")]
    public float MagnetPower;

    public float MagnetDistance;

    public float SuperChargeTime;
    public float SuperDamage;

    public float Speed;

    public float Damage;
    public float FireRate;
    public float CurrentHealth;
    public float MaxHealth;

    [Header("Base Attributes")]
    public float baseDamage;
    public float baseHealth;
    public float baseFireRate;
    public float baseSpecialCountdown;

    public ShipStats(int id, 
        int level,
        float xpToLevel,
        float xp, 
        float currentXp,
        float magnetPower, 
        float magnetDistance,
        float superChargeTime, 
        float superDamage,
        float speed, 
        float damage,
        float fireRate, 
        float currentHealth, 
        float maxHealth, 
        float baseDamage,
        float baseHealth, 
        float baseFireRate, 
        float baseSpecialCountdown)
    {
        this.id = id;
        Level = level;
        this.xpToLevel = xpToLevel;
        this.xp = xp;
        this.currentXp = currentXp;
        MagnetPower = magnetPower;
        MagnetDistance = magnetDistance;
        SuperChargeTime = superChargeTime;
        SuperDamage = superDamage;
        Speed = speed;
        Damage = damage;
        FireRate = fireRate;
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
        this.baseDamage = baseDamage;
        this.baseHealth = baseHealth;
        this.baseFireRate = baseFireRate;
        this.baseSpecialCountdown = baseSpecialCountdown;
    }

    public void SetStats(int level,bool player)
    {
        Level = level;
        var multiplier = level / 10;

        Damage      = Level * baseDamage;
        MaxHealth   = Level * baseHealth;
        FireRate    = baseFireRate;
        Damage      = Level * baseDamage;
        SuperDamage = level * baseDamage;

        if (player)
        {
            xpToLevel = 100 * Mathf.Pow(Level, 0.1f) *
                Mathf.Pow(Level, 2) + Mathf.Pow(Level - 1, 4);

            SuperChargeTime = baseSpecialCountdown;

            float[] UpgradeStats = GetCalculatedUpgradeStats();

            Speed           += UpgradeStats[0];
            Damage          += UpgradeStats[1];
            FireRate        -= UpgradeStats[2];
            MagnetPower     += UpgradeStats[3];
            MagnetDistance  += UpgradeStats[4];
            SuperChargeTime += UpgradeStats[5];
            SuperDamage     += UpgradeStats[6];
        }

        CurrentHealth = MaxHealth;
    }

    public float[] GetCalculatedUpgradeStats()
    {
        var GameControllerSpeedValue = 0;
        var GameControllerDamageValue = 0;
        var GameControllerFireRateValue = 0;
        var GameControllerMagnetPowerValue = 0;
        var GameControllerActivtateDistanceValue = 0;
        var GameControllerSuperTime = 0;
        var GameControllerSuperDamage = 0;
        PlayerData playerData = DataController.GetPlayerData();
        if (GameManager.Instance)
        {
            GameControllerSpeedValue = playerData.Upgrades[(int)UpgradeType.Speed];
            GameControllerDamageValue = playerData.Upgrades[(int)UpgradeType.Damage];
            GameControllerFireRateValue = playerData.Upgrades[(int)UpgradeType.FireRate];
            GameControllerMagnetPowerValue = playerData.Upgrades[(int)UpgradeType.MagnetStrength];
            GameControllerActivtateDistanceValue = playerData.Upgrades[(int)UpgradeType.MagnetDistance];
            GameControllerSuperTime = playerData.Upgrades[(int)UpgradeType.SuperrechargeTime];
            GameControllerSuperDamage = playerData.Upgrades[(int)UpgradeType.SuperDamage];
        }


        var SpeedMultiplier = .1f * GameControllerSpeedValue;
        var DamageMultiplier = 1f * GameControllerDamageValue;
        var FireRateMultiplier = 0.01f * GameControllerFireRateValue;
        var MagnetPowerMultiplier = 1f * GameControllerMagnetPowerValue;
        var MagnetDistanceMultiplier = 1f * GameControllerActivtateDistanceValue;
        var superTime = 0.1f * GameControllerSuperTime;
        var superDamage = 1f * GameControllerSuperDamage;

        return new float[] {
            SpeedMultiplier,
            DamageMultiplier,
            FireRateMultiplier,
            MagnetPowerMultiplier,
            MagnetDistanceMultiplier,superTime,superDamage};
    }


}