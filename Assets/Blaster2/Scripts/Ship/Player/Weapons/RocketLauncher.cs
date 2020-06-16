using TheGamerUrso.PoolSystem;
using UnityEngine;
using TheGamerUrso;

public class RocketLauncher : SpecialAttack
{
    public override void OnUpdate()
    {
        base.OnUpdate();

        PlayerShip playerShip = ship.GetComponent<PlayerShip>();

        if (SpecialActive)
        {
            ActivateSpecial();
            if (m_CountDownTimer == null)
            {
                m_CountDownTimer = new CountDownTimer(SuperChargeTime);
            }

            if (m_CountDownTimer.m_CountdownTimer >= 0)
            {
                m_CountDownTimer.m_CountdownTimer -= Time.deltaTime;
                if (m_CountDownTimer.countToZero())
                {
                    playerData.PowerUpLevel = m_CountDownTimer.m_CountdownTimer / SuperChargeTime;
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

    public override void Shoot()
    {
        if (SpecialActive)
        {
            if (Time.time > newShot)
            {
                newShot = Time.time + GetFireRate();
                GameObject rocket =
                    PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.PlayerRocket);

                rocket.transform.position = transform.position;

                rocket.GetComponent<Rocket>().Damage = Damage;

                PlayWeaponFireSound();

            }
        }
    }


}