using UnityEngine;

public class WeaponController : MonoBehaviour
{
    protected Ship ship;
    [Space(2)]
    [SerializeField] protected BaseWeapon[] Weapons;
    [SerializeField] protected float delayAttak = 3;
    protected bool AutoEnableWeapon;
    protected bool CanAttack;
    protected int currentWeaponActive;

    public int CurrentWeapnType { get; set; }
    public bool ShouldAttack { get; protected set; }

    public void OnEnable()
    {
        currentWeaponActive = 1;
    }

    protected virtual void Start()
    {
        currentWeaponActive = 0;
        DisableAllWeapons();
    }
    protected void Update()
    {
        if(ShouldAttack)
            Attack();
    }

    protected virtual void Attack()
    {

    }

    public virtual void Initialize(Ship ship, float damage, float fireRate)
    {
        this.ship = ship;
        for (int weaponIndex = 0; weaponIndex < Weapons.Length; weaponIndex++)
        {
            Weapons[weaponIndex].Damage = damage;
            Weapons[weaponIndex].FireRate = fireRate;
        }
    }
    public void DisableAllWeapons()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].gameObject.SetActive(false);
        }
    }

    public void SetWeapon(int weaponIndex)
    {
        if (Weapons.Length != 0)
        {
            SwitchWeapon(0);
        }
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

    public virtual void SwitchWeapon(int weaponIndex)
    {
        DisableAllWeapons();
        Weapons[weaponIndex].gameObject.SetActive(true);
    }

    public void SetFireRate(float fireRate = .2f, int weaponIndex = 0, bool all = true)
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

    public void EnableFire()
    {
        ShouldAttack = true;
    }

    public void DisableFire()
    {
        ShouldAttack = false;
    }

    protected virtual void DestroyOwnedProjectiles()
    {
        EnemyProjectile[] enemyProjectiles = GameObject.FindObjectsOfType<EnemyProjectile>();
        if (enemyProjectiles.Length > 0)
        {
            foreach (EnemyProjectile item in enemyProjectiles)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
