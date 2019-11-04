using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class PlayerWeaponSystem : MonoBehaviour
{
    public Action<PlayerWeaponSystem> OnSuperWeapoUsed;

    private int currentWeapon;
    private int clicktimes;
    private float clicktimer;
    private bool clicked;
    [SerializeField] private PlayerWeapon[] Weapons;
    [SerializeField] private SpecialAttack SpecialAttacks = null;

    [Space(2)]
    [Range(1, 4)] private int CurrentWeapnType = 0;
    public static int WeaponUpgradeCollected = 0;
    private Player player;
    private ShipStatsSystem shipStatsSystem;
    private PlayerAnimation playerAnimation;

    [SerializeField] private int superUsed;

    public void ResetSuperUsedToZero()
    {
        superUsed = 0;
    }

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

    public int GetHowManyTimesSuperIsUsed()
    {
        return superUsed;
    }
    public void SetPlayerAnimation(PlayerAnimation playerAnimation)
    {
        this.playerAnimation = playerAnimation;
    }

    public void SetShipStatSystem(ShipStatsSystem shipStatsSystem)
    {
        this.shipStatsSystem = shipStatsSystem;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i].gameObject.activeSelf)
            {
                Weapons[i].SetShipStatsSystem(shipStatsSystem);
                Weapons[i].SetPlayerAnimation(playerAnimation);
                Weapons[i].SetShipTransform(player.transform);
            }
        }

        SpecialAttacks.SetShipStatsSystem(shipStatsSystem);
        SpecialAttacks.SetPlayerAnimation(playerAnimation);
        SpecialAttacks.SetShipTransform(player.transform);
    }

    private void Start()
    {
        PlayerWeaponSystem.SwitchWeapon(this, 0);

    }

    public void IncreasePowerUp(float value)
    {
        SpecialAttacks.IncreasePowerUp(value);
    }

    public float GetPowerUpLevelPresentage()
    {
        return SpecialAttacks.GetPowerUpLevelPresentage();
    }

    public float GetPowerUpCountdown()
    {
        return SpecialAttacks.GetPowerUpCountdown();
    }

    private void Update()
    {
        float playerPowerUp = 0;
        playerPowerUp = GetPowerUpLevelPresentage();
#if UNITY_ANDROID

        if(Time.timeScale == 0)
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

        if (WeaponUpgradeCollected >= 5)
        {
            WeaponUpgradeCollected = 0;
            UpgradeWeapon();
        }


        if (CrossPlatformInputManager.GetButtonDown("Fire2") && playerPowerUp >= 1)
        {
            ActivateSpecial();
        }


    }
    public void UpgradeWeapon()
    {
        if (!PlayerPrefs.HasKey("UpgradeTut"))
        {
            Tutorial.Instance.ShowTutorial(5);

            PlayerPrefs.SetInt("UpgradeTut", 1);
        }
        Player player = GetComponentInParent<Player>();
        if (CurrentWeapnType < 4)
        {
            if (player.CanUsePowerUpItem)
            {
                AudioManager.PlaySound("Power",3);
                CurrentWeapnType++;

                if (CurrentWeapnType > 4)
                {
                    CurrentWeapnType = 4;
                }
            }
            else
            {
                IncreasePowerUp(0.1f);
            }
            Player.TempFireRateUpgrade = false;
            PlayerWeaponSystem.SwitchWeapon(this, CurrentWeapnType);
        }
    }
    public void DownGradeWeapon()
    {
        if (CurrentWeapnType > 0)
        {
            CurrentWeapnType--;
            PlayerWeaponSystem.SwitchWeapon(this, CurrentWeapnType);
        }
    }

    public void ResetWeaponPowerUPCollected()
    {
        WeaponUpgradeCollected = 0;
    }

    public void WeaponPowerUPCollected()
    {
        Player player = GetComponentInParent<Player>();
        if (WeaponUpgradeCollected < 5 && CurrentWeapnType < 4)
        {
            WeaponUpgradeCollected++;

            if (Player.TempFireRateUpgrade == false)
            {
                Player.TempFireRateUpgrade = true;
                player.GiveTemporaryFireRateBuff();
            }
            player.TempFireRateBuff(0.01f * WeaponUpgradeCollected);
        }
    }
    public void ResetWeaponUpgrade()
    {
        CurrentWeapnType = 0;
        PlayerWeaponSystem.SwitchWeapon(this, CurrentWeapnType);
    }

    public void ActivateSpecial()
    {

        SpecialAttacks.ActivateSpecial();
    }

    public void DeactivateSpecial()
    {
        SpecialAttacks.DeactivateSpecial();
    }

    public int getCurrentWeaponType()
    {
        return currentWeapon;
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
}