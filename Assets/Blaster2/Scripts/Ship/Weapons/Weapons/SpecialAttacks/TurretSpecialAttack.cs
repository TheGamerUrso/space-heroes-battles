using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpecialAttack : BaseSpecialAttack
{
    [SerializeField] private PlaceTurrets Turrets;

    public override void OnActivateSpecial()
    {
         Turrets.DeployTurret();
    }

    public override void OnDeactivateSpecial()
    {
            Turrets.DeactiveTurret();
    }
}
