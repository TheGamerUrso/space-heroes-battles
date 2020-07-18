using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireAttack : SpecialAttack
{
    protected PlayerWeapon[] playerWeapons;
    protected float previousRapidFireValue = 0;
    protected int weaponCurrentType;
    protected float playerFireRate;
    private bool RapidFireModeOn;

    public override void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            source.PlayOneShot(weaponData.ShootSFX);

            PlayerData playerData = PersistantData.GetPlayerData();
            playerData.superUsed++;

            if (playerWeapons == null)
            {
                playerWeapons = GameObject.FindObjectsOfType<PlayerWeapon>();
            }

            playerFireRate = playerWeapons[0].weaponData.FireRate;
            weaponCurrentType = ship.GetComponent<PlayerShip>().CurrentWeapnType;

            ship.GetComponent<PlayerShip>().SwitchWeapon(4);

            foreach (PlayerWeapon item in playerWeapons)
            {
                item.weaponData.FireRate = .2f;
            }

            SpecialActive = true;
        }
    }
    public override void DeactivateSpecial()
    {
        if (SpecialActive)
        {
            if (playerWeapons == null)
            {
                playerWeapons = GameObject.FindObjectsOfType<PlayerWeapon>();
            }

            foreach (PlayerWeapon item in playerWeapons)
            {
                item.weaponData.FireRate = playerFireRate;
            }

            ship.GetComponent<PlayerShip>().SwitchWeapon(weaponCurrentType);

            base.DeactivateSpecial();
        }
    }
}
