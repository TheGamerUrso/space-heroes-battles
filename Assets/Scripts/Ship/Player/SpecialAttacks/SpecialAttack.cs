using System;
using TheGamerUrso.PoolSystem;
using UnityEngine;

[Serializable]
public class SpecialAttack : PlayerWeapon
{
    public PlayerData playerData;
    public PlayerShipData playerShipData;

    public int superUsed;
    public int SuperUsed
    {
        get { return superUsed; }
        set { superUsed = value; }
    }

    public bool SpecialActive = false;
    protected CountDownTimer m_CountDownTimer;

    public float superChargeTimer;
    public float SuperChargeTime
    {
        get { return superChargeTimer; }
    }

    public void IncreaseSuperUsed()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        playerData.SuperUsed++;
    }

    public override void OnStart()
    {
        base.OnStart();
        GameSession.SuperUsed = 0;
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        superChargeTimer = playerShipData.SuperChargeTime;
        damage = playerShipData.SuperDamage;
    }

    public virtual void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            AudioManager.PlaySound(null, "Super", 3);

            PlayerData playerData = PersistantData.GetPlayerData();
            playerData.superUsed++;

            SpecialActive = true;
        }
    }

    public virtual void DeactivateSpecial()
    {
        SpecialActive = false;
    }

    public override void OnUpdate()
    {
        PlayerShip playerShip = ship.GetComponent<PlayerShip>();
        if (SpecialActive)
        {
         
            ActivateSpecial();

            if (m_CountDownTimer == null)
                m_CountDownTimer = new CountDownTimer(SuperChargeTime);

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

        float powerLevel = playerShip.GetPowerUpLevelPresentage();

        if (Input.GetKeyDown(KeyCode.F) && powerLevel >= 1)
        {
            ActivateSpecial();
        }
    }

    public override void Shoot()
    {

    }

    public float GetPowerUpCountdown()
    {
        if (m_CountDownTimer != null)
            return m_CountDownTimer.GetPowerUpCountdown();
        else
            return 0;
    }
}