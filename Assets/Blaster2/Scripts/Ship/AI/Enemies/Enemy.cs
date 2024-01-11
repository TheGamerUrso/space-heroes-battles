using System;
using System.Collections;
using UnityEngine;

public class Enemy : Ship, IDamagable, ITargetable
{    
    public static int EnemiesCount;
    public Action OnEnemyAttack;
    public Action<int, int> OnEnemyHit;

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
            return HasShieldModule() || IsAlive;
        }
    }

    [HideInInspector] public EnemyElement enemyElement;

    public override void OnEnable()
    {
        IsAlive = true;
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
        playerData = PersistantData.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();
        SetStats(playerShipData.level);

        HasShield = false;
        ShieldEffect.SetActive(HasShield);
   
        baseEnemyMovement.Speed = Speed;
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
        Events.EnemyGotHit?.Invoke(Id, this);
        OnEnemyHit?.Invoke(hitIndex, numberOfHits);
    }

    public override void Death()
    {
        if (IsAlive)
        {      
            EnemiesCount--;
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
            var ship = other.GetComponent<Ship>();
            ship.TakeDamage(ship.MaxHealth / 2);
            TakeDamage(ship.MaxHealth);
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
        Level = level;

        MaxHealth = Level * EnemyData.baseHealth;

        CurrentHealth = MaxHealth;

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

    public virtual void OnEnemyHitHandled(int hitIndex, int numberOfHits)
    {

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

    }

    public override void ExitLevel()
    {
        Events.EnemyEscaped?.Invoke(Id, this);
    }
}