using System.Collections;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public class PlayerWeaponController : WeaponController
{
    [Header("Weapons")]
    [SerializeField] private BaseSpecialAttack specialAttack;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip powerSFX;
    private IEventService eventService;

    protected override void Start()
    {
        base.Start();
        eventService = GameContext.Get<IEventService>();
    }
    //=================================================================================
    public override void Setup(Ship ship)
    {
        base.Setup(ship);
        specialAttack.Setup(ship);
    }
    //=================================================================================
    public void ActivateSpecial()
    {
        specialAttack.ActivateSpecial();

        eventService.Publish(new PlayerStatsUpdatedEvent() { type = PlayerStatsUpdatedEvent.StatType.SuperUsed, value = 1 });
        eventService?.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.USE, value = 1 });
    }
    //=================================================================================
    public void DeactivateSpecial()
    {
        specialAttack.DeactivateSpecial();
    }
    //=================================================================================
    public BaseSpecialAttack GetSpecialAttack()
    {
        return specialAttack;
    }   
    //=================================================================================
    public void UpdateWeaponStats(float fireRate, float damage = 0)
    {
        var currenActivetWeapon = GetCurrentWeapon().GetComponent<BaseWeapon>();
        currenActivetWeapon.FireRate = fireRate;
        if (damage > 0)
            currenActivetWeapon.Damage = damage;
    }
    //=================================================================================
    public void UpgradeWeapon()
    {
        audioSource.PlayOneShot(powerSFX);
        CurrentWeaponIndex++;

        if (CurrentWeaponIndex > 4)
        {
            CurrentWeaponIndex = 4;
        }
        SwitchWeapon(CurrentWeaponIndex);
    }
    //=================================================================================
    public void DowngradeWeapon()
    {
        CurrentWeaponIndex--;
        if (CurrentWeaponIndex <= 0)
        {
            CurrentWeaponIndex = 0;
        }
        SwitchWeapon(CurrentWeaponIndex);
    }
    //=================================================================================
    public void ResetWeaponUpgrade()
    {
        CurrentWeaponIndex = 0;
        SwitchWeapon(CurrentWeaponIndex);
    }
    //=================================================================================
    public bool IsSuperActive()
    {
        return specialAttack.SpecialActive;
    }
}
