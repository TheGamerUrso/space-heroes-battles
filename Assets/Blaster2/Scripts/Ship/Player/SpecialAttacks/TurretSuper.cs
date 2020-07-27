using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSuper : SpecialAttack
{
    [SerializeField] private float turretDuration;
    [SerializeField] private PlaceTurrets Turrets;

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

            playerData.IncreaseSuperUse(); ;

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
