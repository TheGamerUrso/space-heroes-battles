using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSuper : SpecialAttack
{
    protected PlayerWeapon[] playerWeapons;
    protected int weaponCurrentType;
    public Laser laiser;
    private bool laserOn;
    public float laserSize;
    public LineRenderer lineRenderer;

    public override void ActivateSpecial()
    {
        if (!SpecialActive)
        {
            SpecialActive = true;
            source.PlayOneShot(weaponData.ShootSFX);

            int superUsed = Game.SuperUsed + 1;
            Game.SetSuperUsed(superUsed);
            playerShip.DisableFire();

            laserSize = 0;
            laiser.ActiveLaser();
            lineRenderer.widthMultiplier = laserSize;

     
        }
    }
    public override void DeactivateSpecial()
    {
        if (SpecialActive)
        {
            laserSize = 0;
            laiser.DeactiveLaser();
            lineRenderer.widthMultiplier = laserSize;
            playerShip.EnableFire();

            base.DeactivateSpecial();
        }
    }

    public override void Update()
    {
        if (SpecialActive)
        {
            ActivateSpecial();
            if (m_CountDownTimer == null)
            {
                m_CountDownTimer = new CountDownTimer(SuperChargeTime);
            }

            laserSize = Mathf.Lerp(laserSize, 4, 1);

            if (laserSize > 4)
            {
                laserSize = 4;
            }

            lineRenderer.widthMultiplier = laserSize;

            if (m_CountDownTimer.m_CountdownTimer >= 0)
            {
                if (laserSize == 4)
                {
                    laiser.fullCharge = true;
                    m_CountDownTimer.m_CountdownTimer -= Time.deltaTime;

                    if (m_CountDownTimer.countToZero())
                    {
                        playerData.PowerUpLevel = m_CountDownTimer.m_CountdownTimer / SuperChargeTime;
                    }

                    Shoot();
                }
            }
            else
            {
                laiser.fullCharge = false;
                laiser.DeactiveLaser();
                DeactivateSpecial();
                m_CountDownTimer = null;
                playerData.PowerUpLevel = 0;
            }
        }

        float powerLevel = playerData.GetPowerUpLevelPresentage();

        if (Input.GetKeyDown(KeyCode.F) && powerLevel >= 1)
        {
            ActivateSpecial();
        }
    }
}
