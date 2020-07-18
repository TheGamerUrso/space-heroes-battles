using System;
using UnityEngine;

[Serializable]
public class SpecialAttack : PlayerWeapon
{
    public PlayerData playerData;
    public PlayerShipData playerShipData;
    public PlayerShip playerShip;
    public AudioClip superSFX;

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

    public override void OnStart()
    {
        base.OnStart();
      
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        playerShip = ship.GetComponent<PlayerShip>();
        superChargeTimer = playerShipData.SuperChargeTime;
        damage = playerShipData.SuperDamage;
    }

    public virtual void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            source.PlayOneShot(superSFX);

            if (playerData == null)
            {
                playerData = PersistantData.GetPlayerData();
            }

            int superUsed = GameSession.SuperUsed + 1;
            GameSession.SetSuperUsed(superUsed);

            SpecialActive = true;
        }
    }

    public virtual void DeactivateSpecial()
    {
        SpecialActive = false;
    }

    public override void OnUpdate()
    {
        if (playerShip == null)
        {
            playerShip = ship.GetComponent<PlayerShip>();
        }

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