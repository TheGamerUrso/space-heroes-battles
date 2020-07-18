using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using EasyMobile;
using UnityEngine.Advertisements;

public class MainMenuManager : MonoSingleton<MainMenuManager>
{
    [SerializeField] private TextMeshProUGUI PlayerXPText;
    [SerializeField] private TextMeshProUGUI PlayerLevelText;
    [SerializeField] private ShipSelect shipSelect;

    private int levelIndex;
    private string levelName;

    private PlayerShipData playerShipData;
    private PlayerData playerData;

    public void ShowLeaderboards()
    {
        GooglePlayServicesManager.ShowLeaderboards();
    }

    public void ShowAchievement()
    {
        GooglePlayServicesManager.ShowAchievementa();
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        Events.OnShipSelectValueChanged -= OnShipSelectValueChanged;
        Events.OnXpValueChanged -= XpLevelChanged;
    }

    public void OnShipSelectValueChanged(int selection)
    {
        playerData.currentSelectedShip = selection;
        playerShipData = playerData.GetCurrentPlayerShipData();
        XpLevelChanged(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
    }

    private void Start()
    {
        GameManager.Instance.PauseTheGame(false);
        AudioManager.PlayMusic("Menu");
        Application.targetFrameRate = 30;

        playerData = PersistantData.GetPlayerData();
        playerData.GotHitInGame = false;
        playerData.PlayedGame = false;
        GameSession.SurvivalMode = false;

        playerShipData = playerData.GetCurrentPlayerShipData();
        Events.OnXpValueChanged += XpLevelChanged;

        Events.OnShipSelectValueChanged += OnShipSelectValueChanged;
        XpLevelChanged(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);


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
        if (playerShipData.level >= playerData.MaxLevel)
        {
            PlayerLevelText.text = "" + playerShipData.level;
            PlayerXPText.text = "-/-";
        }
        else
        {
            PlayerLevelText.text = "" + playerShipData.level;

            PlayerXPText.text = string.Format("{0}/{1}",
              Mathf.Round(playerShipData.xp),
                Mathf.Round(playerShipData.xpToLevel));
        }

        LevelValueChanged(lvl);
    }

    public void QuitButtonEvent()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        levelIndex = GameManager.LevelIndexSelected;
        GameManager.Instance.LoadScene((LevelEnum)(levelIndex + 1));
    }



}