using TheGamerUrso.PoolSystem;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{
    [Header("Ship Config")]
    [SerializeField] protected ShipStatsSystem shipStatsSystem;
    [SerializeField] protected LevelSystem levelSystem;
    protected Animator animator;

    protected bool HasShield;

    [Header("Effects")]
    [SerializeField] protected GameObject ShieldEffect;
    [SerializeField] protected PoolGameObjectType ExplostionEffect;

    #region Getters and Setters
    public int Level
    {
        get
        {
            return levelSystem.GetLevel();
        }
    }

    public float MaxHealth
    {
        get
        {
            return shipStatsSystem.GetMaxHealth();
        }
        set { shipStatsSystem.SetMaxHealth(value); }
    }

    public float CurrentHealth
    {
        get { return shipStatsSystem.currentHealth; }
        set
        {
            shipStatsSystem.currentHealth = value;
            shipStatsSystem.HealthChanged?.Invoke(shipStatsSystem.currentHealth, MaxHealth);
        }
    }
    #endregion

    public Animator GetAnimator()
    {
        return animator;
    }

    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        ShipSetup();
    }

    public abstract void ShipSetup();
    public abstract void OnAwake();
    public abstract void Death();

    //Get Health Presentatge
    public float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth)*100;
    }

    public virtual void InstallShieldModule()
    {
        if (HasShield)
        {
            return;
        }

        HasShield = true;
    }

    public bool HasShieldModule()
    {
        return HasShield;
    }

    //Setter   and Getter for LevelSystem
    public LevelSystem GetLevelSystem()
    {
        return levelSystem;
    }
    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;
    }

    //Getter and Setter for ShipStatSystem
    public ShipStatsSystem GetShipStatsSystem()
    {
        return shipStatsSystem;
    }
    public void SetShipStatSystem(ShipStatsSystem newshipStatsSystem)
    {
        shipStatsSystem = newshipStatsSystem;
    }

    public void IncreasePowerUp(float value)
    {
        GetShipStatsSystem().PowerUpLevel += value;
    }

    public float GetPowerUpLevelPresentage()
    {
        return GetShipStatsSystem().PowerUpLevel;
    }

}