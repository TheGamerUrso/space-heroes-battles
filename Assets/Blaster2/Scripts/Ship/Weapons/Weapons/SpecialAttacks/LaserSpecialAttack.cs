using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpecialAttack : BaseSpecialAttack
{
    protected DefaultPlayerWeapon[] playerWeapons;
    protected int weaponCurrentType;
    public Laser laiser;
    private bool laserOn;
    public float laserSize;
    public LineRenderer lineRenderer;

    public override void OnActivateSpecial()
    {
        ship.DisableFire();
        laserSize = 0;
        laiser.ActiveLaser();
        lineRenderer.widthMultiplier = laserSize;
    }
    public override void OnDeactivateSpecial()
    {
        laiser.fullCharge = false;
        ship.EnableFire();
        laserSize = 0;
        laiser.DeactiveLaser();
        lineRenderer.widthMultiplier = laserSize;
    }

    public override void Update()
    {
        if (SpecialActive)
        {
            laserSize = Mathf.Lerp(laserSize, 4, 1);

            if (laserSize > 4)
            {
                laserSize = 4;
            }

            lineRenderer.widthMultiplier = laserSize;
        }
        if (laserSize == 4)
        {
            laiser.fullCharge = true;
            base.Update();
        }
    }

    public void LaiserWidth()
    {
        laserSize = Mathf.Lerp(laserSize, 4, 1);

        if (laserSize > 4)
        {
            laserSize = 4;
        }

        lineRenderer.widthMultiplier = laserSize;
    }

    public override void Shoot()
    {
        base.Shoot();
    }
}
