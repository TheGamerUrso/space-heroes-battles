using TheGamerUrso.PoolSystem;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{
    [Header("Ship Config")]
    [SerializeField] protected ShipStatsSystem shipStatsSystem;
    [SerializeField] protected LevelSystem levelSystem;
    protected Animator animator;

    protected bool bShieldModuleInstalled;

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
        InitReferences();
    }

    private void Start()
    {
        ShipSetup();
    }

    public abstract void ShipSetup();
    public abstract void InitReferences();
    public abstract void Death();

    //Get Health Presentatge
    public float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth)*100;
    }

    public virtual void InstallShieldModule()
    {
        if (bShieldModuleInstalled)
        {
            return;
        }

        bShieldModuleInstalled = true;
    }

    public bool HasShieldModule()
    {
        return bShieldModuleInstalled;
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


}