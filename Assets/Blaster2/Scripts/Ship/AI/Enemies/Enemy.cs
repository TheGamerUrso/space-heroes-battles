using System;
using System.Collections;
using UnityEngine;

public class Enemy : Ship, IDamagable, ITargetable
{
    public enum EnemyType
    {
        SimpleEnemy, EnemyWithWeapons, Boss1, Boss2, Boss3, Boxer
    }

    public virtual event Action<float, float> OnHealthChanged;
    public Action OnEnemyAttack;
    public Action<int, int> OnEnemyHit;

    #region Components
    [SerializeField] private EnemyType enemyType;
    protected BaseEnemyMovement baseEnemyMovement;
    #endregion


    public string Id;
    public Enemy_SO EnemyData;


    #region Stats

    [Header("STATS")]
    public int Level;
    public float Damage;
    public float FireRate;
    public float Speed;
    public float currentHealth;
    public float maxHealth;
    public float CurrentHealth { get { return currentHealth; } }
    public float MaxHealth { get { return maxHealth; } }
    #endregion

    #region Weapons
    [Space(2)]
    [SerializeField] protected WeaponScript[] Weapons;
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

    #region Health

    public bool IsAlive { get; set; }
    public virtual EnemyHealthWidget HealthBar { get; set; }
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
            return HasShieldModule() || IsAlive;
        }
    }

    [HideInInspector] public EnemyElement enemyElement;

    public override void OnEnable()
    {
        IsAlive = true;
        Game.NumberOfEnemies++;
        currentWeaponActive = 1;
    }


    public override void Awake()
    {
        OnEnemyHit = OnEnemyHitHandled;
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
        baseEnemyMovement = GetComponent<BaseEnemyMovement>();
    }

    public override void Start()
    {
        HasShield = false;

        playerData = PersistantData.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();

        SetEnemyHealthUI();

        ShieldEffect.SetActive(HasShield);

        SetStats(playerShipData.level);

        baseEnemyMovement.Speed = Speed;

        currentWeaponActive = 0;

        DisableAllWeapons();

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
            HealthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            HealthBar.Setup(this, false);
        }
    }

    public override void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
    }

    public virtual void Leave()
    {
        Events.EnemyEscaped?.Invoke(Id, this);
    }

    public virtual void TakeDamage(float dmg)
    {
        if (IsAlive == false)
        {
            return;
        }

        AudioManager.PlaySound(EnemyData.hitSFX);

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
                currentHealth -= dmg;

                OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

                if (currentHealth < 1)
                {
                    Death();
                }
            }

        }
        Hit();
    }

    public virtual void Hit()
    {
        hitIndex++;
        Events.EnemyGotHit?.Invoke(Id, this);
        OnEnemyHit?.Invoke(hitIndex, numberOfHits);
    }

    public virtual void Death()
    {
        if (IsAlive)
        {
            IsAlive = false;
            var explostion = PoolManager.Instance.GetObjectFromPool(EnemyData.ExplostionEffect);
            explostion.transform.position = transform.position;
            explostion.SetActive(true);
            Events.EnemyDied?.Invoke(Id, this);
            HealthBar.Hide();
            gameObject.SetActive(false);

            Events.ShakeCamera?.Invoke(.5f);

            DropItem.Instance.PickRandomDropItem(transform);
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
            var destroyable = other.GetComponent<IDamagable>();
            destroyable.TakeDamage(destroyable.MaxHealth / 2);
        }
    }
    public void SetFireRate(int weaponIndex = 0, bool all = true)
    {
        if (all)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                float newFireRate = Weapons[i].FireRate - .2f;

                Weapons[i].FireRate = newFireRate;
            }
        }
        else
        {
            float newFireRate = Weapons[weaponIndex].FireRate - .2f;

            Weapons[weaponIndex].FireRate = newFireRate;
        }
    }

    public virtual void AddDamagablePart(IDamagable part)
    {

    }

    public override void SetStats(int level)
    {
        Level = level;

        maxHealth = Level * EnemyData.baseHealth;

        currentHealth = MaxHealth;

        Speed = EnemyData.baseSpeed;

        Damage = Level * EnemyData.baseDamage;

        FireRate = EnemyData.baseFireRate;

        HasShield = false;

        for (int weaponIndex = 0; weaponIndex < Weapons.Length; weaponIndex++)
        {
            Weapons[weaponIndex].Damage = Damage;
            Weapons[weaponIndex].FireRate = FireRate;
        }

    }

    public float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth) * 100;
    }

    public virtual void Heal(float ammount)
    {

    }

    public void EnableWeaponById(int id, bool solo = false)
    {
        if (solo)
        {
            DisableAllWeapons();
        }

        Weapons[id].AutoAttack = true;
    }

    public void EnableAllWeapon()
    {
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

    public void SetHealth(float health)
    {
        currentHealth = health;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public virtual void OnEnemyHitHandled(int hitIndex, int numberOfHits)
    {

    }

    protected virtual void DestroyOwnedProjectiles()
    {

    }

    public virtual IEnumerator DelayStart()
    {
        HealthBar.Show();
        yield return new WaitForSeconds(2);
        EnableColliders(true);
        if (enemyType != EnemyType.SimpleEnemy)
        {
            EnableWeaponById(0);
        }
    }
}