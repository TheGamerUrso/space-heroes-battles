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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip powerSFX;
    private IEventService eventService;

    protected override void Start()
    {
        base.Start();
        eventService = GameContext.Get<IEventService>();
    }
    //=================================================================================
    public void Setup(Ship ship, PlayerData playerData, PlayerShipData playerShipData)
    {
        Setup(ship, playerShipData.Damage, playerShipData.FireRate);
        SwitchWeapon(0);
        specialAttack.Setup(ship, playerData, playerShipData);
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
}
