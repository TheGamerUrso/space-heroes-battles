using System;
using UnityEngine;

[Serializable]
public class BaseSpecialAttack : BaseWeapon
{
    [SerializeField] protected PlayerData playerData;
    [SerializeField] protected PlayerShipData playerShipData;
    public bool SpecialActive { get; protected set; } = false;

    protected CountDownTimer m_CountDownTimer;
    private PlayerShip playerShip;
    public override void Start()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
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

                if (ship.GetHealthPresentage() <= .5f)
                {
                    ship.Heal(.1f);
                }

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