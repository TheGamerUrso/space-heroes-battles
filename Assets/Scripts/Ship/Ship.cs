using TheGamerUrso.PoolSystem;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{
    [Header("Ship Config")]
    protected GameManager gameManager;
    [SerializeField] protected ShipStatsSystem shipStatsSystem;
    [SerializeField] protected LevelSystem levelSystem;
    [SerializeField] protected GameObject ShieldEffect;
    [SerializeField] protected PoolGameObjectType ExplostionEffect;

    [SerializeField] protected Animator animator;
    protected bool bShieldModuleInstalled;

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

    public float CurrentHealth{get
        {
            return shipStatsSystem.CurrentHealth;
        }
        set
        {
            shipStatsSystem.CurrentHealth = value;
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
        ShipStartSetUp();
    }

    public abstract void ShipStartSetUp();
    public abstract void InitReferences();
    public abstract void Death();

    //Get Health Presentatge
    public float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth)*100;
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
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