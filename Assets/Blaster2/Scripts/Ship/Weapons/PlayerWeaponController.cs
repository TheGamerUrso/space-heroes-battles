using System.Collections;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public class PlayerWeaponController : WeaponController
{

    [SerializeField] protected PlayerData playerData;
    [SerializeField] protected PlayerShipData playerShipData;
    [Header("Weapons")]
    [SerializeField] private BaseSpecialAttack specialAttack;
    private IEventService eventService;
    public AudioSource audioSource;
    public AudioClip powerSFX;

    protected override void Start()
    {
        base.Start();
        eventService = GameContext.Get<IEventService>();
    }
    public override void Setup(Ship Ship,float damage,float fireRate)
    {
        base.Setup(ship, damage, fireRate);
        SwitchWeapon(0);
        specialAttack.Setup(Ship,playerData, playerShipData);
    }
    //=================================================================================
    public void ActivateSpecial()
    {
        eventService?.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.USE, value = playerData.SuperUsed });
        playerData.SuperUsed++;
        specialAttack.ActivateSpecial();
    }
    //=================================================================================
    public void DeactivateSpecial()
    {
        specialAttack.DeactivateSpecial();
    }
    //=================================================================================
    public GameObject GetCurrentActiveWeapon()
    {
        return Weapons[CurrentWeapnType].gameObject;
    }
    //=================================================================================
    public BaseSpecialAttack GetSpecialAttack()
    {
        return specialAttack;
    }   
    //=================================================================================
    public void UpdateWeaponStats(float fireRate, float damage = 0)
    {
        var currenActivetWeapon = GetCurrentActiveWeapon().GetComponent<BaseWeapon>();
        currenActivetWeapon.FireRate = fireRate;
        if (damage > 0)
            currenActivetWeapon.Damage = damage;
    }
    //=================================================================================
    public void UpgradeWeapon()
    {
        audioSource.PlayOneShot(powerSFX);
        CurrentWeapnType++;

        if (CurrentWeapnType > 4)
        {
            CurrentWeapnType = 4;
        }
        SwitchWeapon(CurrentWeapnType);
    }
    //=================================================================================
    public void DowngradeWeapon()
    {
        CurrentWeapnType--;
        if (CurrentWeapnType <= 0)
        {
            CurrentWeapnType = 0;
        }
        SwitchWeapon(CurrentWeapnType);
    }
    //=================================================================================
    public void ResetWeaponUpgrade()
    {
        CurrentWeapnType = 0;
        SwitchWeapon(CurrentWeapnType);
    }
}
