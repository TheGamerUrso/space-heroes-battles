using System;
using TheGamerUrso.PoolSystem;
using UnityEngine;

[Serializable]
public class SpecialAttack : PlayerWeapon
{
    public Action<int> OnSuperWeapoUsed;
    public int superUsed { get; set; }

    public bool SpecialActive = false;
    private CountDownTimer m_CountDownTimer;
    private float previousRapidFireValue = 0;
    private int weaponCurrentType;
    private float playerFireRate;
    private float turretDuration;

    private PlayerWeapon[] playerWeapons;

    public void IncreaseSuperUsed()
    {
        PlayerData playerData = DataController.GetPlayerData();
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Use);
        superUsed++;
        if (objectiveData != null)
        {
            objectiveData.UpdateProgress(superUsed);
        }
    }

    private void Start()
    {
        superUsed = 0;
        playerShip = GetComponentInParent<PlayerShip>();
    }

    public void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            AudioManager.PlaySound(null, "Super", 3);

            OnSuperWeapoUsed?.Invoke(superUsed);

            if (RapidFireMoade)
            {
                if (playerWeapons == null)
                {
                    playerWeapons = GameObject.FindObjectsOfType<PlayerWeapon>();
                }

                playerFireRate = playerWeapons[0].FireRate;
                weaponCurrentType = playerShip.getCurrentWeaponType;

                playerShip.SwitchWeapon(4);

                foreach (PlayerWeapon item in playerWeapons)
                {
                    item.FireRate = 0.2f;
                }
            }



            if (weaponData.SummonTurrets)
            {
                turretDuration = SuperChargeTime;
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

                playerShip.SwitchWeapon(weaponCurrentType);
            }

            SpecialActive = false;
        }
    }
    public override void OnUpdate()
    {
        if (playerShip == null)
        {
            playerShip = PlayerManager.GetPlayer();
        }

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
                        playerShip.PowerUpLevel = m_CountDownTimer.m_CountdownTimer / turretDuration;
                    }
                    else
                    {
                        playerShip.PowerUpLevel = m_CountDownTimer.m_CountdownTimer / SuperChargeTime;
                    }
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
                playerShip.PowerUpLevel = 0;
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

                    rocket.GetComponent<Rocket>().Damage = Damage;

                    rocket.GetComponent<Rocket>().HomeMissleType = weaponData.m_HomeMissleUpgrade;

                    PlayWeaponFireSound();

                }
            }
        }
    }

    public float GetPowerUpCountdown()
    {
        if (m_CountDownTimer != null)
            return m_CountDownTimer.GetPowerUpCountdown();
        else
            return 0;
    }
}