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

    private PlayerShipData playerShipData = new PlayerShipData();
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
#if UNITY_EDITOR
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals("boot"))
            {
                Debug.Log("boot found skip");
                return;
            }
            Debug.Log("Boot not found Loading");
            SceneManager.LoadScene("boot", LoadSceneMode.Additive);
        }

#endif
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        PlayerData playerData = DataController.GetPlayerData();
        playerData.OnShipSelectValueChanged -= OnShipSelectValueChanged;

        playerShipData = playerData.GetCurrentPlayerShipData();
        playerData.OnXpValueChanged -= XpLevelChanged;
     

    }

    public void OnShipSelectValueChanged(int selection)
    {
        PlayerData playerData = DataController.GetPlayerData();
        playerData.currentSelectedShip = selection;
        playerShipData = playerData.GetCurrentPlayerShipData();
        XpLevelChanged(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
    }

    private void Start()
    {
        //version.text = "ver " + Application.version;
        GameManager.PauseTheGame(false);
        AudioManager.PlayMusic("Menu");
        PlayerData playerData = DataController.GetPlayerData();
        playerData.GotHitInGame = false;
        playerData.PlayedGame = false;

        Application.targetFrameRate = 30;

        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();
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
            PlayerData playerData = DataController.GetPlayerData();
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