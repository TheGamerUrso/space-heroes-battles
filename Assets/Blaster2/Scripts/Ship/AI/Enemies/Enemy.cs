using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public class Enemy : Ship, IDamagable, ITargetable
{    
    public event Action<Enemy> OnEnemyDied;
    public event Action<Enemy,int,int> OnEnemyHit;
    public event Action<Enemy> OnEnemyAttack;
    public event Action<Enemy> OnEnemyEscaped;
    public event Action<Enemy> OnEnemyEntered;

    #region Components
    protected BaseEnemyMovement baseEnemyMovement;
    #endregion

    public Enemy_SO EnemyData;

    #region Weapons
    [Space(2)]
    [SerializeField] protected BaseWeapon[] Weapons;
    [SerializeField] protected float delayAttak = 3;
    protected bool AutoEnableWeapon;
    protected bool CanAttack;
    protected int currentWeaponActive;
    #endregion

    #region Collision
    protected BoxCollider boxCollider;
    protected float takeDamageDelay;
    protected int hitIndex;
    protected int numberOfHits;
    #endregion

    protected PlayerData playerData;

    public GameObject target
    {
        get
        {
            return gameObject;
        }
    }

    public bool Targetable
    {
        get
        {
            return HasShield || IsAlive;
        }
    }

    [HideInInspector] public EnemyElement enemyElement;
    protected IDataService dataService;
    protected IEventService eventService;

    public override void OnEnable()
    {
        IsAlive = true;
        currentWeaponActive = 1;
    }


    public override void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
        baseEnemyMovement = GetComponent<BaseEnemyMovement>();
    }

    public override void Start()
    {
        eventService = GameContext.Get<IEventService>();
        audioService = GameContext.Get<IAudioService>();
        dataService = GameContext.Get<IDataService>();
        playerData = dataService.GetPlayerData();

        HasShield = false;
        ShieldEffect.SetActive(HasShield);

        float difficultyMultiplier = 0.1f * GameController.Instance.difficulty;
        difficultyMultiplier = Mathf.Max(0.1f, difficultyMultiplier); // Correctly checks the variable
       
        baseEnemyMovement.Speed = Speed + difficultyMultiplier;
        difficultyMultiplier = Mathf.Max(0.1f, difficultyMultiplier); // Correctly checks the variable


        currentWeaponActive = 0;


        DisableAllWeapons();

        SetEnemyHealthUI();
        HealthBar.Show();

        StartCoroutine(DelayStart());
    }

    public virtual void SetEnemyHealthUI()
    {
        if (HealthBar != null)
        {
            HealthBar.GetComponent<BaseHealthWidget>();
        }
        if (EnemyData.HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(EnemyData.HealthBarSettings.HealthBarPrefab, transform, false);
            HealthBar = initializedHealthWidget.GetComponent<EnemyHealthWidget>();
            HealthBar.GetComponent<EnemyHealthWidget>().Setup(this,true);
        }
    }

    public override void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
    }

    public override void TakeDamage(float dmg)
    {
        if (IsAlive == false)
        {
            return;
        }

        audioService.PlaySound(EnemyData.hitSFX);

        if (takeDamageDelay <= 0)
        {
            takeDamageDelay = .1f;

            if (HasShield == true)
            {
                HasShield = false;
                ShieldEffect.SetActive(HasShield);
            }
            else if (HasShield == false)
            {
                CurrentHealth -= dmg;

                OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

                if (CurrentHealth < 1)
                {
                    Death();
                }
            }

        }
        Hit();
    }

    public override void Hit()
    {
        hitIndex++;
    }

    public override void Death()
    {
        if (IsAlive)
        {      
            IsAlive = false;
            var explostion = PoolManager.Instance.GetObjectFromPool(EnemyData.ExplostionEffect);
            explostion.transform.position = transform.position;
            explostion.SetActive(true);

            OnEnemyDied?.Invoke(this);

            HealthBar.Hide();
            gameObject.SetActive(false);

            eventService.Publish(new ShakeCameraEvent());
            eventService.Publish(new DropRandomItemEvent() { SpawnPosition = transform });
        }
    }

    public void EnableColliders(bool enabled)
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = enabled;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            var ship = other.GetComponent<Ship>();
            ship.TakeDamage(ship.MaxHealth / 2);
            TakeDamage(MaxHealth);
        }
    }

    public void SetFireRate(float fireRate = .2f,int weaponIndex = 0, bool all = true)
    {
        if (all)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                float newFireRate = Weapons[i].FireRate - fireRate;

                Weapons[i].FireRate = newFireRate;
            }
        }
        else
        {
            float newFireRate = Weapons[weaponIndex].FireRate - fireRate;

            Weapons[weaponIndex].FireRate = newFireRate;
        }
    }

    public override void SetStats(int level)
    {
        Level = Mathf.Clamp(level, 1, 10);

        // Using a 25% health increase and 15% damage increase per level
        float healthGrowthRate = 0.25f;
        float damageGrowthRate = 0.18f;

        MaxHealth = EnemyData.baseHealth * (1f + (healthGrowthRate * (Level - 1)));
        CurrentHealth = MaxHealth;

        Speed = EnemyData.baseSpeed; // Keep speed consistent per archetype

        Damage = EnemyData.baseDamage * (1f + (damageGrowthRate * (Level - 1)));
        FireRate = EnemyData.baseFireRate;

        HasShield = false;

        for (int weaponIndex = 0; weaponIndex < Weapons.Length; weaponIndex++)
        {
            Weapons[weaponIndex].Damage = Damage;
            Weapons[weaponIndex].FireRate = FireRate;
        }
    }

    public override float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth) * 100;
    }

    public override void Heal(float ammount)
    {

    }

    public override void SwitchWeapon(int id, bool solo = false)
    {
        if (solo)
        {
            DisableAllWeapons();
        }

        Weapons[id].AutoAttack = true;
    }

    public void EnableAllWeapon()
    {     
        Debug.Log(gameObject.name + "");
        if (Weapons.Length > 0)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].AutoAttack = true;
            }
        }
    }

    public void DisableAllWeapons()
    {
        if (Weapons.Length > 0)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].AutoAttack = false;
            }
        }
    }

    protected virtual void DestroyOwnedProjectiles()
    {

    }

    public virtual IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(2);
        EnableColliders(true);
        if (Weapons.Length != 0)
        {
            SwitchWeapon(0);
        }
    }

    public override void EnterLevel()
    {
        OnEnemyEntered?.Invoke(this);
    }

    public override void ExitLevel()
    {
        OnEnemyEscaped?.Invoke(this);
    }
}