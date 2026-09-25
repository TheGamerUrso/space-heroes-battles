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

    public bool TempFireRateUpgrade { get; set; }
    public bool CanUsePowerUpItem { get; set; }

    public AudioSource audioSource;
    public AudioClip powerSFX;

    protected override void Start()
    {
        base.Start();
        eventService = GameContext.Get<IEventService>();
    }

    public override void Initialize(Ship Ship,float damage,float fireRate)
    {
        base.Initialize(ship, damage, fireRate);
        SwitchWeapon(0);
        specialAttack.Initialize(Ship,playerData, playerShipData);
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
    public void UpgradeWeapon()
    {
        if (CurrentWeapnType < 4)
        {
            if (CanUsePowerUpItem)
            {
                audioSource.PlayOneShot(powerSFX);
                CurrentWeapnType++;

                if (CurrentWeapnType > 4)
                {
                    CurrentWeapnType = 4;
                }
            }
            else
            {
                playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
            }

            TempFireRateUpgrade = false;
            SwitchWeapon(CurrentWeapnType);
        }
    }
    public void DowngradeWeapon()
    {
        if (playerShipData.HasArmorUpgrade)
        {
            return;
        }

        if (CurrentWeapnType > 0)
        {
            CurrentWeapnType--;
            SwitchWeapon(CurrentWeapnType);
        }
    }
    //=================================================================================
    public void ResetWeaponUpgrade()
    {
        CurrentWeapnType = 0;
        SwitchWeapon(CurrentWeapnType);
    }
    //=================================================================================
    public void TempFireRateBuff(float fireRate = 0.0f, bool temporary = false)
    {

        if (!TempFireRateUpgrade)
        {
            TempFireRateUpgrade = true;
            GiveTemporaryFireRateBuff();
        }


        UpdateWeaponStats(playerShipData.FireRate - fireRate);
    }
    //=================================================================================

    public void GiveTemporaryFireRateBuff()
    {
        StartCoroutine(TemporaryFireRateUpgrade());
    }
    //=================================================================================
    public IEnumerator TemporaryFireRateUpgrade()
    {
        var fireRateTemp = playerShipData.FireRate;
        var DamageTemp = playerShipData.Damage;

        while (TempFireRateUpgrade)
        {
            yield return new WaitForEndOfFrame();
        }

        playerShipData.FireRate = fireRateTemp;
        UpdateWeaponStats(playerShipData.FireRate, DamageTemp);
    }

}
