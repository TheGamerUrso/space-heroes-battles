using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpecialAttack : BaseSpecialAttack
{
    [SerializeField] private PlaceTurrets Turrets;

    public override void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            source.PlayOneShot(weaponData.ShootSFX);

            playerData.IncreaseSuperUse(); ;

            Turrets.DeployTurret();

            SpecialActive = true;
        }
    }
    public override void DeactivateSpecial()
    {
        if (SpecialActive)
        {
            Turrets.DeactiveTurret();
            base.DeactivateSpecial();
        }
    }
}
