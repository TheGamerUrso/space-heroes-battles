using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSuper : SpecialAttack
{
    public float turretDuration;
    public PlaceTurrets Turrets;

    public override void Start()
    {
        base.Start();
        turretDuration = playerShipData.SuperChargeTime;
    }

    public override void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            source.PlayOneShot(weaponData.ShootSFX);

            playerData.superUsed++;

            turretDuration = SuperChargeTime;
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
