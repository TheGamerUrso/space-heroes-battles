using System;
using UnityEngine;

[Serializable]
public class BaseSpecialAttack : BaseWeapon
{
    public bool SpecialActive { get; protected set; } = false;
    protected CountDownTimer m_CountDownTimer;
    public float SuperChargeTime { get; set; }

    public override void Update()
    {
        if (SpecialActive)
        {
            var playerShipData = ((PlayerShip)ship).GetPlayerShipData();
            if (playerShipData == null) return;

            if (m_CountDownTimer == null)
            {
                m_CountDownTimer = new CountDownTimer(SuperChargeTime);
            }
               

            if (m_CountDownTimer.m_CountdownTimer >= 0)
            {
                m_CountDownTimer.m_CountdownTimer -= Time.deltaTime;
                if (m_CountDownTimer.countToZero())
                {
                    playerShipData.SetSuperCharge(m_CountDownTimer.m_CountdownTimer / SuperChargeTime);
                }

                var damagable = ship.GetComponent<IDamagable>();
                damagable.Heal(.1f);

                Shoot();
            }
            else
            {
                DeactivateSpecial();
                m_CountDownTimer = null;
                playerShipData.SetSuperCharge(0);
            }
        }
    }

    public virtual void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            SpecialActive = true;
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

    public override void UpdateStats()
    {
        var playerShipData = ((PlayerShip)ship).GetPlayerShipData();
        if (playerShipData == null) return;

        Damage = playerShipData.SuperDamage;
        FireRate = playerShipData.FireRate;
        SuperChargeTime = playerShipData.SuperChargeTime;
    }

    public override void Shoot()
    {

    }
}