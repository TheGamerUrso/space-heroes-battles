using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSuper : SpecialAttack
{
    public float turretDuration;
    public PlaceTurrets Turrets;

    public override void OnStart()
    {
        base.OnStart();
        turretDuration = playerShipData.SuperChargeTime;
    }

    public override void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            source.PlayOneShot(superSFX);

            playerData.superUsed++;

            turretDuration = SuperChargeTime;
            Turrets.CreateTurret();

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

    public override void OnUpdate()
    {
        base.OnUpdate();

        PlayerShip playerShip = ship.GetComponent<PlayerShip>();

        if (SpecialActive)
        {
            ActivateSpecial();
            if (m_CountDownTimer == null)
            {
                m_CountDownTimer = new CountDownTimer(turretDuration);
            }

            if (m_CountDownTimer.m_CountdownTimer >= 0)
            {
                m_CountDownTimer.m_CountdownTimer -= Time.deltaTime;
                if (m_CountDownTimer.countToZero())
                {
                    playerData.PowerUpLevel = m_CountDownTimer.m_CountdownTimer / turretDuration;
                }

                if (playerShip.HealthPresentage <= .5f)
                {
                    playerShip.Heal(.1f);
                }

                Shoot();
            }
            else
            {
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
