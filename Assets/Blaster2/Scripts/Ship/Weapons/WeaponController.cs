using System.Linq;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] protected Ship ship;
    [Space(2)]
    [SerializeField] protected BaseWeapon[] Weapons;
    [SerializeField] protected float delayAttak = 3;
    protected bool AutoEnableWeapon;
    protected bool CanAttack;
    public int CurrentWeaponIndex { get; set; } = 0;
    public bool ShouldAttack { get; set; }

    protected virtual void Start()
    {
        CurrentWeaponIndex = 0;
        DisableAllWeapons();
    }
    //=================================================================================
    public virtual void Setup(Ship ship)
    {
        this.ship = ship;
        for (int weaponIndex = 0; weaponIndex < Weapons.Length; weaponIndex++)
        {
            Weapons[weaponIndex].Setup(ship);
        }
        SetWeapon(CurrentWeaponIndex);
    }
    //=================================================================================
    public BaseWeapon GetCurrentWeapon()
    {
        // Check if the list is null or completely empty
        if (Weapons == null || Weapons.Length == 0)
        {
            return null;
        }

        // Clamp the index to the highest available element if it exceeds the bounds
        if (CurrentWeaponIndex >= Weapons.Length)
        {
            CurrentWeaponIndex = Weapons.Length - 1;
        }

        return Weapons[CurrentWeaponIndex];
    }
    //=================================================================================

    public void DisableAllWeapons()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].gameObject.SetActive(false);
        }
    }
    //=================================================================================
    public void SetWeapon(int weaponIndex)
    {
        if (Weapons.Length == 0) return;
        SwitchWeapon(weaponIndex);
    }  
    //=================================================================================
    public void EquipRandomWeapon()
    {
        if (Weapons.Length == 0) return;
        CurrentWeaponIndex = (UnityEngine.Random.Range(0, Weapons.Length));
        SwitchWeapon(CurrentWeaponIndex);
    }
    //=================================================================================
    public virtual void SwitchWeapon(int weaponIndex)
    {
        DisableAllWeapons();
        Weapons[weaponIndex].gameObject.SetActive(true);
    }
    //=================================================================================
    public void SetFireRate(float fireRate = .2f, int weaponIndex = 0, bool all = true)
    {
        if (all)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].FireRate = fireRate;
            }
        }
        else
        {
            Weapons[weaponIndex].FireRate = fireRate;
        }
    }
    //=================================================================================
    public void EnableFire()
    {
        ShouldAttack = true;
    }
    //=================================================================================
    public void DisableFire()
    {
        ShouldAttack = false;
    }
    //=================================================================================
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
