using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireSpecialAttack : BaseSpecialAttack
{
    protected DefaultPlayerWeapon[] playerWeapons;
    protected float previousRapidFireValue = 0;
    protected int weaponCurrentType;
    protected float playerFireRate;
    private bool RapidFireModeOn;

    public override void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            source.PlayOneShot(weaponData.ShootSFX);

            playerData.IncreaseSuperUse();        

            playerFireRate = playerWeapons[0].FireRate;

            weaponCurrentType = ship.CurrentWeapnType;

            ship.SwitchWeapon(4);

            for (int i = 0; i < playerWeapons.Length; i++)
            {
                playerWeapons[i].FireRate = .2f;
            }

            SpecialActive = true;
        }
    }
    public override void DeactivateSpecial()
    {
        if (SpecialActive)
        {
            for (int i = 0; i < playerWeapons.Length; i++)
            {
                playerWeapons[i].FireRate = playerFireRate;
            }

            ship.SwitchWeapon(weaponCurrentType);

            base.DeactivateSpecial();
        }
    }
}
