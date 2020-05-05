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
        if (SpecialActive == false)
        {
            AudioManager.PlaySound(null, "Super", 3);
            if (playerData == null)
            {
                playerData = PersistantData.GetPlayerData();
            }
            playerData.superUsed++;
            laserSize = 0;
            laiser.ActiveLaser();
            lineRenderer.widthMultiplier = laserSize;
            SpecialActive = true;
            GameSession.CanFire = false;
        }
    }
    public override void DeactivateSpecial()
    {
        if (SpecialActive)
        {
            laserSize = 0;
            laiser.DeactiveLaser();
            lineRenderer.widthMultiplier = laserSize;
            GameSession.CanFire = true;
            base.DeactivateSpecial();
        }
    }

    public override void OnUpdate()
    {
        PlayerShip playerShip = ship.GetComponent<PlayerShip>();
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
