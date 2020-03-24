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
    [SerializeField] private WeaponScript RocketLauncher = null;
    [Space(2)]
    [Range(1, 4)] private int CurrentWeapnType = 0;
    public static int WeaponUpgradeCollected = 0;
    private PlayerShip playerShip;
    public ShipStatsSystem ShipStatsSystem { get { 
            return playerShip.GetShipStatsSystem(); } }

    [SerializeField] private int superUsed;


    public SpecialAttack GetSpecialAttack()
    {
        return SpecialAttacks;
    }
    
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

    public void SetPlayer(PlayerShip player)
    {
        this.playerShip = player;
        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i].gameObject.activeSelf)
            {
                Weapons[i].SetShipTransform(player.transform);
            }
        }

        SpecialAttacks.SetShipTransform(player.transform);
    }
    private void OnDestroy()
    {
        if (playerShip == null)
            playerShip = GetComponentInParent<PlayerShip>();

        if (playerShip != null)
            playerShip.PlayerShipHit -= DownGradeWeapon;
    }



    private void Start()
    {
        if (playerShip == null)
            playerShip = GetComponentInParent<PlayerShip>();

        if (playerShip != null)
            playerShip.PlayerShipHit += DownGradeWeapon;

        SwitchWeapon(this, 0);


        GameEventSystem.BossHitEvent += BossHitEventCallback;
    }

    public void BossHitEventCallback(string id,BaseBossEnemy bossEnemy,float ammount)
    {
        IncreasePowerUp(ammount);
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


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerWeaponSystem playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
            playerWeaponSystem.IncreasePowerUp(0.5f);
            playerWeaponSystem.WeaponPowerUPCollected();
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
        PlayerShip player = GetComponentInParent<PlayerShip>();
        if (CurrentWeapnType < 4)
        {
            if (player.CanUsePowerUpItem)
            {
                AudioManager.PlaySound(null,"Power",3);
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
            PlayerShip.TempFireRateUpgrade = false;
            PlayerWeaponSystem.SwitchWeapon(this, CurrentWeapnType);
        }
    }
    public void DownGradeWeapon()
    {
        if (playerShip.HasArmorUpgrade() == false)
        {
            if (CurrentWeapnType > 0)
            {
                CurrentWeapnType--;
                PlayerWeaponSystem.SwitchWeapon(this, CurrentWeapnType);
            }
        }
    }

    public void ResetWeaponPowerUPCollected()
    {
        WeaponUpgradeCollected = 0;
    }

    public void WeaponPowerUPCollected()
    {

        if (WeaponUpgradeCollected <= 5 && CurrentWeapnType < 4)
        {
            WeaponUpgradeCollected+=2;
            if (WeaponUpgradeCollected > 5)
            {
                WeaponUpgradeCollected = 5;
            }
            Debug.Log("increase FireRate by " + 0.01f * WeaponUpgradeCollected);
            playerShip.TempFireRateBuff(0.01f * WeaponUpgradeCollected);
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
}