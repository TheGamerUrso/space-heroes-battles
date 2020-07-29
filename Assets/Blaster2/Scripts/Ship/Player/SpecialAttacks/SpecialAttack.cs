using System;
using UnityEngine;

[Serializable]
public class SpecialAttack : PlayerWeapon
{
    public bool SpecialActive { get; protected set; } = false;

    protected CountDownTimer m_CountDownTimer;

    public override void Start()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        playerShip = ship.GetComponent<PlayerShip>();

        SuperChargeTime = playerShipData.SuperChargeTime;
        Damage = playerShipData.SuperDamage;
        FireRate = playerShipData.FireRate;
    }

    public override void Update()
    {
        if (SpecialActive)
        {
            if (m_CountDownTimer == null)
                m_CountDownTimer = new CountDownTimer(SuperChargeTime);

            if (m_CountDownTimer.m_CountdownTimer >= 0)
            {
                m_CountDownTimer.m_CountdownTimer -= Time.deltaTime;
                if (m_CountDownTimer.countToZero())
                {

                    playerData.PowerUpLevel = m_CountDownTimer.m_CountdownTimer / SuperChargeTime;

                }

                if (playerShip.GetHealthPresentage() <= .5f)
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

    public virtual void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            source.PlayOneShot(weaponData.ShootSFX);

            int superUsed = Game.SuperUsed + 1;
            Game.SetSuperUsed(superUsed);

            SpecialActive = true;
        }
    }
    public virtual void DeactivateSpecial()
    {
        SpecialActive = false;
    }
    public float GetPowerUpCountdown()
    {
        if (m_CountDownTimer != null)
            return m_CountDownTimer.GetPowerUpCountdown();
        else
            return 0;
    }

    public override void SetStats(PlayerShipData playerShipData)
    {
        Damage = playerShipData.SuperDamage;
        SuperChargeTime = playerShipData.SuperChargeTime;
    }
}