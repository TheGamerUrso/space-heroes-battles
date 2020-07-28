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
    [SerializeField] private TextMeshProUGUI PlayerLevelText;
    [SerializeField] private ShipSelect shipSelect;

    private Level level;
    private string levelName;

    private PlayerShipData playerShipData;
    private PlayerData playerData;

    [SerializeField] private Mission currentMission;


    public void ShowLeaderboards()
    {
        GooglePlayServicesManager.ShowLeaderboards();
    }

    public void ShowAchievement()
    {
        GooglePlayServicesManager.ShowAchievementa();
    }

    private void Start()
    {
        GameManager.Instance.PauseTheGame(false);
        AudioManager.PlayMusic("Menu");
        Application.targetFrameRate = 30;

        playerData = PersistantData.GetPlayerData();
        playerData.GotHitInGame = false;
        playerData.PlayedGame = false;
        Game.IsSurvivalMode = false;


        if (PlayerPrefs.HasKey("SurvivalMode"))
        {
            if (playerData.SurvivalUnlocked)
            {
                int announceModUnlocked = PlayerPrefs.GetInt("SurvivalMode");
                if (announceModUnlocked == 0)
                {
                    PlayerPrefs.SetInt("SurvivalMode", 1);

                    Notification notification = new Notification();
                    notification.Description = "Survival Mode Unlocked";

                    NotificationSystem.Instance.Add(notification);
                }
            }
        }

        shipSelect.SetShipTexture(playerData.CurrrentSelectedShip);
    }


    public void QuitButtonEvent()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        level = GameManager.Instance.GetCurrentLevelSelected();
        GameManager.Instance.LoadScene((LevelEnum)level.mission.ID);
    }




}