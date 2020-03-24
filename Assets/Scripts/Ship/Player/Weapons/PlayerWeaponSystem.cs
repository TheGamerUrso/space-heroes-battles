using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class PlayerWeaponSystem : MonoBehaviour
{
  
    private PlayerShip playerShip;

  
    [SerializeField] private PlayerWeapon[] Weapons;
    [SerializeField] private SpecialAttack specialAttack = null;
    [SerializeField] private WeaponScript rocketLauncher = null;   
    public int getCurrentWeaponType
    {
        get { return currentWeapon; }
    }
    public WeaponScript RocketLauncher
    {
        get
        {
            return rocketLauncher;
        }
    }
    public SpecialAttack SpecialAttack {
        get
        {
            return specialAttack;
        }
    
    }


    [Space(2)]
    [Range(1, 4)] private int CurrentWeapnType = 0;
    public static int PowerUpCollectAmmount = 0;


    private int currentWeapon;
    private int clicktimes;
    private float clicktimer;
    private bool clicked;


    private void OnDestroy()
    {
        if (playerShip != null)
            playerShip.PlayerShipHit -= DownGradeWeapon;
    }

    private void Start()
    {
        playerShip = GetComponent<PlayerShip>();

        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i].gameObject.activeSelf)
            {
                Weapons[i].SetShipTransform(playerShip.transform);
            }
        }

        specialAttack.SetShipTransform(playerShip.transform);

        playerShip.PlayerShipHit += DownGradeWeapon;

        SwitchWeapon(this, 0);
    }

    private void Update()
    {
        float playerPowerUp = 0;
        if (playerShip != null)
        {
            playerPowerUp = playerShip.GetPowerUpLevelPresentage();
        }

#if UNITY_ANDROID

        if (Time.timeScale == 0)
        {
            clicked = false;
            clicktimer = 1;
            clicktimes = 0;
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (playerPowerUp >= 1)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        clicked = true;
                        clicktimer = 1;
                        clicktimes++;
                        break;

                    case TouchPhase.Moved:
                        break;

                    case TouchPhase.Stationary:
                        break;

                    case TouchPhase.Ended:
                        break;

                    case TouchPhase.Canceled:
                        break;

                    default:
                        break;
                }

                if (clicked && clicktimer > 0)
                {
                    clicktimer -= Time.deltaTime;
                    if (clicktimer <= 0)
                    {
                        clicked = false;
                        clicktimer = 1;
                        clicktimes = 0;
                    }
                }

                if (clicktimes > 1 || touch.tapCount > 2)
                {
                    clicktimes = 0;
                    ActivateSpecial();
                }
            }
        }
#endif

        if (PowerUpCollectAmmount >= 5)
        {
            PowerUpCollectAmmount = 0;
            UpgradeWeapon();
        }


        if (CrossPlatformInputManager.GetButtonDown("Fire2") && playerPowerUp >= 1)
        {
            ActivateSpecial();
        }


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerShip playerShip = PlayerManager.GetPlayer();
            if (playerShip != null)
            {
                playerShip.IncreasePowerUp(0.5f);
                PowerUpCollected();
            }
        }
#endif

    }
    public void UpgradeWeapon()
    {
        if (!PlayerPrefs.HasKey("UpgradeTut"))
        {
            Tutorial.Instance.ShowTutorial(5);

            PlayerPrefs.SetInt("UpgradeTut", 1);
        }

        if (CurrentWeapnType < 4)
        {
            if (playerShip.CanUsePowerUpItem)
            {
                AudioManager.PlaySound(null, "Power", 3);
                CurrentWeapnType++;

                if (CurrentWeapnType > 4)
                {
                    CurrentWeapnType = 4;
                }
            }
            else
            {
                playerShip.IncreasePowerUp(0.1f);
            }

            PlayerShip.TempFireRateUpgrade = false;
            SwitchWeapon(this, CurrentWeapnType);
        }
    }

    public void DownGradeWeapon()
    {
        if (playerShip.HasArmorUpgrade == false)
        {
            if (CurrentWeapnType > 0)
            {
                CurrentWeapnType--;
                SwitchWeapon(this, CurrentWeapnType);
            }
        }
    }

    public void ResetWeaponPowerUPCollected()
    {
        PowerUpCollectAmmount = 0;
    }

    public void PowerUpCollected()
    {

        if (PowerUpCollectAmmount <= 5 && CurrentWeapnType < 4)
        {
            PowerUpCollectAmmount += 2;
            if (PowerUpCollectAmmount > 5)
            {
                PowerUpCollectAmmount = 5;
            }
            Debug.Log("increase FireRate by " + 0.01f * PowerUpCollectAmmount);
            playerShip.TempFireRateBuff(0.01f * PowerUpCollectAmmount);
        }
    }

    public void ResetWeaponUpgrade()
    {
        CurrentWeapnType = 0;
        SwitchWeapon(this, CurrentWeapnType);
    }

    public void ActivateSpecial()
    {

        specialAttack.ActivateSpecial();
    }

    public void DeactivateSpecial()
    {
        specialAttack.DeactivateSpecial();
    }

    public static GameObject GetCurrentActiveWeapon(PlayerWeaponSystem weaponSystem)
    {
        return weaponSystem.Weapons[weaponSystem.currentWeapon].gameObject;
    }

    public static void SwitchWeapon(PlayerWeaponSystem weaponSystem, int WeaponTypeIndex)
    {
        weaponSystem.currentWeapon = WeaponTypeIndex;

        for (int i = 0; i < weaponSystem.Weapons.Length; i++)
        {
            weaponSystem.Weapons[i].gameObject.SetActive(false);
        }

        weaponSystem.Weapons[WeaponTypeIndex].gameObject.SetActive(true);
    }



    #region Get and Set
    public SpecialAttack GetSpecialAttack()
    {
        return specialAttack;
    }

    #endregion
}