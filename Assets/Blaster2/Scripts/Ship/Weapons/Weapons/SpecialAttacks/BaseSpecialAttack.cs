using System;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

[Serializable]
public class BaseSpecialAttack : BaseWeapon
{
    protected PlayerData playerData;
    protected PlayerShipData playerShipData;
    public bool SpecialActive { get; protected set; } = false;

    protected CountDownTimer m_CountDownTimer;

    public void Initialize(Ship ship, PlayerData playerData, PlayerShipData playerShipData)
    {
        this.playerData = playerData;
        this.playerShipData = playerShipData;
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
                    playerData.SetSuperMeter(m_CountDownTimer.m_CountdownTimer / SuperChargeTime);

                }

                var damagable = ship.GetComponent<IDamagable>();
                damagable.Heal(.1f);

                Shoot();
            }
            else
            {
                DeactivateSpecial();
                m_CountDownTimer = null;
                playerData.SetSuperMeter(0);
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
            SpecialActive = true;
            playerData.SetUsedSuperCount(1);
    
            source.PlayOneShot(weaponData.ShootSFX);
            OnActivateSpecial();
        }
    }

    public virtual void OnActivateSpecial(){}
    public virtual void OnDeactivateSpecial(){}

    public virtual void DeactivateSpecial()
    {
        if (SpecialActive)
        {
            SpecialActive = false;
            OnDeactivateSpecial();
        }
    }
    public float GetPowerUpCountdown()
    {
        if (m_CountDownTimer != null)
            return m_CountDownTimer.GetPowerUpCountdown();
        else
            return 0;
    }

    public void SetStats(PlayerShipData playerShipData, int weaponType = 1)
    {
        Damage = playerShipData.SuperDamage;
        SuperChargeTime = playerShipData.SuperChargeTime;
        FireRate = playerShipData.FireRate;
    }

    public override void Shoot()
    {

    }
}