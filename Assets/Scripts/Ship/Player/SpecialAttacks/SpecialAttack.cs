using System;
using UnityEngine;

[Serializable]
public class SpecialAttack : PlayerWeapon
{
    private bool SpecialActive = false;
    private float SuperTimer;
    private CountDownTimer m_CountDownTimer;
    private float PowerUpLevel = 0;
    private float previousRapidFireValue = 0;
    private int weaponCurrentType;
    private float playerFireRate;

    private PlayerWeaponSystem playerWeaponSystem;
    private PlayerWeapon[] playerWeapons;

    public void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            AudioManager.PlaySound("Super", 3);

            if (playerWeaponSystem == null)
            {
                playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
            }



            playerWeaponSystem.IncreaseSuperUsed();

            if (weaponData.RapidFireMode)
            {
  

                if (playerWeapons == null)
                {
                    playerWeapons = GameObject.FindObjectsOfType<PlayerWeapon>();
                }

                playerFireRate = playerWeapons[0].weaponData.m_FireRate;
                weaponCurrentType = playerWeaponSystem.getCurrentWeaponType();

                PlayerWeaponSystem.SwitchWeapon(playerWeaponSystem, 4);
                
                foreach (PlayerWeapon item in playerWeapons)
                {
                    item.weaponData.m_FireRate = 0.2f;
                }
            }

            SuperTimer = shipStatsSystem.baseSpecialCountdown;

            if (weaponData.SummonTurrets)
            {
                GetComponentInChildren<PlaceTurrets>().CreateTurret();
                SuperTimer = GetComponentInChildren<PlaceTurrets>().TurretPrefab.GetComponent<Turret>().TTL;
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
                    item.weaponData.m_FireRate = playerFireRate;
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
                m_CountDownTimer = new CountDownTimer(SuperTimer);
            }

            if (m_CountDownTimer.m_CountdownTimer >= 0)
            {
                m_CountDownTimer.m_CountdownTimer -= Time.deltaTime;
                if (m_CountDownTimer.countToZero())
                {
                    PowerUpLevel = m_CountDownTimer.m_CountdownTimer / SuperTimer;
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
                if (Time.time > m_NewShot)
                {
                    m_NewShot = Time.time + weaponData.m_FireRate;
                    GameObject rocket = 
                        PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.PlayerRocket);

                    rocket.transform.position = transform.position;

                    rocket.GetComponent<Rocket>().setDamage(weaponData.m_WeaponDamage);

                    rocket.GetComponent<Rocket>().HomeMissleType = weaponData.m_HomeMissleUpgrade;

                    AudioManager.PlaySound(source, weaponData.ShootSoundEffect);

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

    public override void InitWeapon()
    {
        SuperTimer = shipStatsSystem.SuperChargeTime;
        weaponData.m_WeaponDamage = shipStatsSystem.SuperDamage;
        weaponData.m_FireRate = shipStatsSystem.FireRate;

    }

}