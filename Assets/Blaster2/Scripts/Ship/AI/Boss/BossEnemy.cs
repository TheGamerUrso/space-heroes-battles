using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossEnemy : Ship, IDamagable, ITargetable
{
    public enum BossEnemyType
    {
        Boss1, Boss2, Boss3, Boxer
    }

    [SerializeField] private BossEnemyType bossType;
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
    [HideInInspector] public EnemyElement enemyElement;
    [SerializeField] private BossEnemyMove bossEnemyMove;
    [SerializeField] private WeaponScript[] Weapons;
    [SerializeField] private float delayAttak = 3;
    private bool AutoEnableWeapon;
    private BoxCollider boxCollider;
    private bool CanAttack;
    private int currentWeaponActive;
    private float takeDamageDelay;

    [SerializeField] private EnemyHealthWidget healthbar;
    public EnemyHealthWidget HealthBar
    {
        get
        {
            return healthbar;
        }
        set
        {
            healthbar = value;
        }
    }

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

    private PlayerData playerData;
    public Action OnBossAttack;
    public Action<int, int> OnBossHit;

    protected int hitIndex;
    protected int numberOfHits;

    [SerializeField] protected List<IDamagable> DestroyableParts = new List<IDamagable>();

    public override void OnEnable()
    {
        IsAlive = true;
        EnableColliders(false);
        currentWeaponActive = 1;
        Game.NumberOfEnemies++;
    }
    public override void Awake()
    {
        OnBossHit = OnBossHitHandled;
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
        bossEnemyMove = GetComponent<BossEnemyMove>();
    }

    public override void Start()
    {
        base.Start();

        HasShield = false;

        playerData = PersistantData.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();

        healthbar.Setup(this, false);

        ShieldEffect.SetActive(HasShield);

        SetStats(playerShipData.level);

        bossEnemyMove.Speed = Speed;

        currentWeaponActive = 0;

        DisableAllWeapons();
    }

    public void TakeDamage(float dmg)
    {
        if (GuiManager.Instance.IsTrasnmiting() || delayAttak > 0)
        {
            return;
        }

        if (IsAlive == false)
        {
            return;
        }

        int destroyed = 0;

        for (int i = 0; i < DestroyableParts.Count; i++)
        {
            IDamagable part = DestroyableParts[i];
            if (part.CurrentHealth <= 0)
            {
                destroyed++;
            }
        }

        if (DestroyableParts.Count > 0)
        {
            if (destroyed < DestroyableParts.Count)
            {
                HasShield = true;
                if (!ShieldEffect.activeInHierarchy)
                {
                    ShieldEffect.SetActive(true);
                }
            }
            else
            {
                HasShield = false;
            }

        }
        else
        {
            HasShield = false;
            ShieldEffect.SetActive(HasShield);
        }

        AudioManager.PlaySound(EnemyData.hitSFX);

        if (takeDamageDelay <= 0)
        {
            takeDamageDelay = .1f;

            if (HasShield == true)
            {
                HasShield = false;
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


    public void Leave()
    {

    }
    public void Hit()
    {
        hitIndex++;
        OnBossHit?.Invoke(hitIndex, numberOfHits);
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.15f);
    }

    public override void Update()
    {
        base.Update();
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }

        if (CurrentHealth > 0)
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (delayAttak > 0)
        {
            delayAttak -= Time.deltaTime;
        }
        else
        {
            OnBossAttack?.Invoke();
        }
    }

    public void Death()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        animator.SetBool("Death", true);

        StartCoroutine(DeathSequence());

        EnableColliders(false);

        DisableAllWeapons();

        EnemyProjectile[] enemyProjectiles = GameObject.FindObjectsOfType<EnemyProjectile>();
        if (enemyProjectiles.Length > 0)
        {
            foreach (EnemyProjectile item in enemyProjectiles)
            {
                item.gameObject.SetActive(false);
            }
        }

        if (HealthBar != null)
        {
            Destroy(HealthBar);
        }

        Events.BossDied?.Invoke(Id, this);


    }

    IEnumerator DeathSequence()
    {
        if (CurrentHealth < 0)
        {
            EnableColliders(false);

            Vector3[] positions ={
                 transform.position,
                transform.position + (transform.right * 50),
                  transform.position - (transform.right * 50),
                    transform.position + (transform.forward * 50),
                      transform.position - (transform.forward * 50)
            };

            for (int i = 0; i < 5; i++)
            {
                var explostion = PoolManager.Instance.GetObjectFromPool(EnemyData.ExplostionEffect);
                explostion.transform.position = positions[i];
                explostion.SetActive(true);
            }
        }

        yield return new WaitForSeconds(4.0f);

        for (int i = 0; i < 4; i++)
        {
             DropItem.Instance.PickRandomDropItem(transform);
        }

        if (IsAlive)
        {
            IsAlive = false;
            var explostion = PoolManager.Instance.GetObjectFromPool(EnemyData.ExplostionEffect);
            explostion.transform.position = transform.position;
            explostion.SetActive(true);
            Events.BossDied?.Invoke(Id, this);
            HealthBar.Hide();


             DropItem.Instance.PickRandomDropItem(transform);

            Destroy(transform.parent.gameObject);
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

    public void AddDamagablePart(IDamagable part)
    {
        DestroyableParts.Add(part);

        MonoBehaviour go = part as MonoBehaviour;
        if (go != this)
        {
            BossDestroyablePart partGO = go.GetComponent<BossDestroyablePart>();
            partGO.maxHealth = part.MaxHealth / 2;
            float health = part.MaxHealth;
            partGO.SetHealth(health);
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

    public void OnBossHitHandled(int hitIndex, int numberOfHits)
    {
        if (GuiManager.Instance.IsTrasnmiting())
        {
            return;
        }

        PlayerData playerData = PersistantData.GetPlayerData();

        if (playerData != null)
        {
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.05f);
        }
    }
}
