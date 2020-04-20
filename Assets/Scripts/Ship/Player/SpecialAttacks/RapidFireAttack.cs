using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireAttack : SpecialAttack
{

    protected PlayerWeapon[] playerWeapons;
    protected float previousRapidFireValue = 0;
    protected int weaponCurrentType;
    protected float playerFireRate;
    private bool RapidFireModeOn;
    public override void ActivateSpecial()
    {
        if (SpecialActive == false)
        {
            AudioManager.PlaySound(null, "Super", 3);

            PlayerData playerData = PersistantData.GetPlayerData();
            playerData.superUsed++;

            if (playerWeapons == null)
            {
                playerWeapons = GameObject.FindObjectsOfType<PlayerWeapon>();
            }

            playerFireRate = playerWeapons[0].FireRate;
            weaponCurrentType = ship.GetComponent<PlayerShip>().getCurrentWeaponType;

            ship.GetComponent<PlayerShip>().SwitchWeapon(4);

            foreach (PlayerWeapon item in playerWeapons)
            {
                item.FireRate = 0.2f;
            }

            SpecialActive = true;
        }
    }
    public override void DeactivateSpecial()
    {
        if (SpecialActive)
        {
            if (playerWeapons == null)
            {
                playerWeapons = GameObject.FindObjectsOfType<PlayerWeapon>();
            }

            foreach (PlayerWeapon item in playerWeapons)
            {
                item.SetFireRate(playerFireRate);
            }

            ship.GetComponent<PlayerShip>().SwitchWeapon(weaponCurrentType);

            base.DeactivateSpecial();
        }
    }

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


            }
        }
    }

}
