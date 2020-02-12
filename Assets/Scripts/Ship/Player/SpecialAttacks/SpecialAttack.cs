using System;
using TheGamerUrso.PoolSystem;
using UnityEngine;

[Serializable]
public class SpecialAttack : PlayerWeapon
{
    private bool SpecialActive = false;
    private CountDownTimer m_CountDownTimer;
    private float PowerUpLevel = 0;
    private float previousRapidFireValue = 0;
    private int weaponCurrentType;
    private float playerFireRate;
    private float turretDuration;

    private PlayerWeaponSystem playerWeaponSystem;
    private PlayerWeapon[] playerWeapons;

    public void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            AudioManager.PlaySound(null,"Super", 3);

            if (playerWeaponSystem == null)
            {
                playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
            }



            playerWeaponSystem.IncreaseSuperUsed();

            if (RapidFireMoade)
            {
                if (playerWeapons == null)
                {
                    playerWeapons = GameObject.FindObjectsOfType<PlayerWeapon>();
                }

                playerFireRate = playerWeapons[0].FireRate;
                weaponCurrentType = playerWeaponSystem.getCurrentWeaponType();

                PlayerWeaponSystem.SwitchWeapon(playerWeaponSystem, 4);
                
                foreach (PlayerWeapon item in playerWeapons)
                {
                    item.FireRate = 0.2f;
                }
            }

            turretDuration = SuperChargeTime;

            if (weaponData.SummonTurrets)
            {
                GetComponentInChildren<PlaceTurrets>().CreateTurret();
                turretDuration = GetComponentInChildren<PlaceTurrets>().TurretPrefab.GetComponent<Turret>().TTL;
            }

            SpecialActive = true;
        }
    }

    public void DeactivateSpecial()
    {
        if (SpecialActive)
        {
            if (weaponData.RapidFireMode)
            {
                if (playerWeapons == null)
                {
                    playerWeapons = GameObject.FindObjectsOfType<PlayerWeapon>();
                }
                foreach (PlayerWeapon item in playerWeapons)
                {
                    item.SetFireRate(playerFireRate);
                }

                if (playerWeaponSystem == null)
                {
                    playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
                }

                PlayerWeaponSystem.SwitchWeapon(playerWeaponSystem,weaponCurrentType);
            }

            SpecialActive = false;
        }
    }

    public override void Update()
    {
        base.Update();

        if (SpecialActive)
        {
            ActivateSpecial();
            if (m_CountDownTimer == null)
            {
                if (weaponData.SummonTurrets)
                {
                    m_CountDownTimer = new CountDownTimer(turretDuration);
                }
                else
                {
                    m_CountDownTimer = new CountDownTimer(SuperChargeTime);
                }         
            }

            if (m_CountDownTimer.m_CountdownTimer >= 0)
            {
                m_CountDownTimer.m_CountdownTimer -= Time.deltaTime;
                if (m_CountDownTimer.countToZero())
                {
                    if (weaponData.SummonTurrets)
                    {
                        PowerUpLevel = m_CountDownTimer.m_CountdownTimer / turretDuration;
                    }
                    else
                    {
                        PowerUpLevel = m_CountDownTimer.m_CountdownTimer / SuperChargeTime;
                    }
                }

                if (PlayerManager.GetPlayer().GetHealthPresentage() <= .5f)
                {
                    PlayerManager.GetPlayer().Heal(.1f);
                }

            }
            else
            {
                DeactivateSpecial();
                m_CountDownTimer = null;
                PowerUpLevel = 0;
            }
        }

        float powerLevel = GetPowerUpLevelPresentage();

        if (Input.GetKeyDown(KeyCode.F) && powerLevel >= 1)
        {
            ActivateSpecial();
        }
    }

    public override void Shoot()
    {
        if (weaponData.SuperRockFireMode)
        {
            if (SpecialActive)
            {
                if (Time.time > newShot)
                {
                    newShot = Time.time + GetFireRate();
                    GameObject rocket = 
                        PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.PlayerRocket);

                    rocket.transform.position = transform.position;

                    rocket.GetComponent<Rocket>().setDamage(Damage);

                    rocket.GetComponent<Rocket>().HomeMissleType = weaponData.m_HomeMissleUpgrade;

                    PlayWeaponFireSound();

                }
            }
        }
    }

    public void IncreasePowerUp(float value)
    {
        if (SpecialActive)
        {
            return;
        }

        PowerUpLevel += value;
    }

    public float GetPowerUpLevelPresentage()
    {
        return PowerUpLevel;
    }

    public float GetPowerUpCountdown()
    {
        if (m_CountDownTimer != null)
            return m_CountDownTimer.GetPowerUpCountdown();
        else
            return 0;
    }
}