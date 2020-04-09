using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using TheGamerUrso.SceneLoader;
using UnityEngine.SceneManagement;

public class MainMenuManager : Singleton<MainMenuManager>
{
    public TextMeshProUGUI PlayerXPText;
    public TextMeshProUGUI PlayerLevelText;

    [SerializeField] private TextMeshProUGUI version;


    private int levelIndex;
    private string levelName;

    private PlayerShipData playerShipData;

    public void ShowProfile()
    {

    }

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
        //version.text = "ver " + Application.version;
        GameManager.PauseTheGame(false);
        AudioManager.PlayMusic("Menu");
        Application.targetFrameRate = 30;
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        PlayerData playerData =  GameManager.Instance.GetPlayerData();
        playerData.OnShipSelectValueChanged -= OnShipSelectValueChanged;
        playerData.OnXpValueChanged -= XpLevelChanged;
    }

    public void OnShipSelectValueChanged(int selection)
    {
        PlayerData playerData =  GameManager.Instance.GetPlayerData();
        playerData.currentSelectedShip = selection;
        playerShipData = playerData.GetCurrentPlayerShipData();
        XpLevelChanged(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
    }

    private void Start()
    {
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        playerData.GotHitInGame = false;
        playerData.PlayedGame = false;


        playerShipData = playerData.GetCurrentPlayerShipData();
        playerData.OnXpValueChanged += XpLevelChanged;

        playerData.OnShipSelectValueChanged += OnShipSelectValueChanged;
        XpLevelChanged(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
    }

    public void LevelValueChanged(int lvl)
    {
        PlayerLevelText.text = string.Format("Level \n {0}",lvl);

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
            PlayerData playerData =  GameManager.Instance.GetPlayerData();
            playerData.EarnXP(100);
        }
    }
    public void QuitButtonEvent()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        levelIndex = GameManager.LevelSelected;
        levelName = string.Format("Level" + (levelIndex + 1));
        SceneLoader.Instance.LoadScene(levelName);

    }



}