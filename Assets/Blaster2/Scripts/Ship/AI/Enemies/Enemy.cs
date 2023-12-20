using System;
using System.Collections;
using UnityEngine;

public class Enemy : Ship, IDamagable, ITargetable
{
    public enum EnemyType
    {
        SimpleEnemy, EnemyWithWeapons
    }

    [SerializeField] private EnemyType enemyType;

    public event Action<float, float> OnHealthChanged;

    public string Id;
    public Enemy_SO EnemyData;
    public bool IsAlive { get; set; }

    [Header("STATS")]
    public int Level;
    public float Damage;
    public float FireRate;
    public float Speed;
    public float currentHealth;
    public float maxHealth;
    public float CurrentHealth { get { return currentHealth; } }
    public float MaxHealth { get { return maxHealth; } }

    [Space(2)]
    [SerializeField] protected WeaponScript[] Weapons;
    [SerializeField] protected float delayAttak = 3;
    protected bool AutoEnableWeapon;
    protected BoxCollider boxCollider;
    protected bool CanAttack;
    protected int currentWeaponActive;
    protected float takeDamageDelay;
    protected EnemyMove enemyMove;

    public EnemyHealthWidget HealthBar { get; set; }

    public GameObject target {
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
    }


    public override void Awake()
    {
        HasShield = false;
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
        enemyMove = GetComponent<EnemyMove>();
    }

    public override void Start()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();

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

        ShieldEffect.SetActive(HasShield);

        SetStats(playerShipData.level);

        enemyMove.Speed = Speed;

        if (enemyType == EnemyType.EnemyWithWeapons)
        {
            StartCoroutine(ActivateWeapons());
        }
    }

    public override void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
    }

    public void Leave()
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
        Events.EnemyGotHit?.Invoke(Id, this);
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

    //Enemy With Weapon Type Methods
    IEnumerator ActivateWeapons()
    {
        yield return new WaitForSeconds(2);

        EnableWeaponById(0);

        if (HealthBar != null)
        {
            HealthBar.Show();
        }

        EnableColliders(true);
    }
}