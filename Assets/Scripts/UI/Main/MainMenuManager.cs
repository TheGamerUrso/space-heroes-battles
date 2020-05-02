using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using TheGamerUrso.SceneLoader;
using UnityEngine.SceneManagement;
using EasyMobile;
using UnityEngine.Advertisements;

public class MainMenuManager : Singleton<MainMenuManager>
{
    public TextMeshProUGUI PlayerXPText;
    public TextMeshProUGUI PlayerLevelText;

    private int levelIndex;
    private string levelName;

    private PlayerShipData playerShipData;
    private PlayerData playerData;

    public ShipSelect shipSelect;

    public void ShowLeaderboards()
    {
        GooglePlayServicesManager.Instance.ShowLeaderboards();
    }

    public void ShowAchievement()
    {
        GooglePlayServicesManager.Instance.ShowAchievementa();
    }
    protected override void OnAwake()
    {
        base.OnAwake();

        GameManager.Instance.PauseTheGame(false);
        AudioManager.PlayMusic("Menu");
        Application.targetFrameRate = 30;
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        playerData.OnShipSelectValueChanged -= OnShipSelectValueChanged;
        playerData.OnXpValueChanged -= XpLevelChanged;
    }

    public void OnShipSelectValueChanged(int selection)
    {
        playerData.currentSelectedShip = selection;
        playerShipData = playerData.GetCurrentPlayerShipData();
        XpLevelChanged(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
    }

    private void Start()
    {
        AdvertismentManager.HideBanner();
        playerData = PersistantData.GetPlayerData();
        playerData.GotHitInGame = false;
        playerData.PlayedGame = false;
        GameSession.SurvivalMode = false;

        playerShipData = playerData.GetCurrentPlayerShipData();
        playerData.OnXpValueChanged += XpLevelChanged;

        playerData.OnShipSelectValueChanged += OnShipSelectValueChanged;
        XpLevelChanged(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);

        ShipSelect.Instance.Initialize();
        if (PlayerPrefs.HasKey("SurvivalMode"))
        {
            if (playerData.SurvivalUnlocked)
            {
                int announceModUnlocked = PlayerPrefs.GetInt("SurvivalMode");
                if (announceModUnlocked == 0)
                {
                    PlayerPrefs.SetInt("SurvivalMode", 1);
                    Popup.Show(Popup.popupType.error, "Survival Mode Unlocked", false);
                }
            }
        }
    }

    public void LevelValueChanged(int lvl)
    {
        PlayerLevelText.text = string.Format("{0}", lvl);

    }

    public void XpLevelChanged(int lvl, float xp, float xpToLevel)
    {
        if (playerShipData.level >= playerShipData.MaxLevel)
        {
            PlayerLevelText.text = "" + playerShipData.level;
            PlayerXPText.text = "Maxed";
        }
        else
        {
            PlayerLevelText.text = "" + playerShipData.level;

            PlayerXPText.text = string.Format("{0}/{1}",
             playerShipData.xp,
                Mathf.Round(playerShipData.xpToLevel));
        }

        LevelValueChanged(lvl);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerData.EarnXP(100);
        }
    }
    public void QuitButtonEvent()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        levelIndex = GameManager.LevelIndexSelected;
        levelName = string.Format("Level" + (levelIndex + 1));
        SceneLoader.Instance.LoadScene(levelName);
    }



}