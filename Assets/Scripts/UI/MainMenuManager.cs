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
    private void Start()
    {
        version.text = "ver " + Application.version;
        GameManager.PauseTheGame(false);
        AudioManager.PlayMusic("Menu");
        PlayerData playerData = DataController.GetPlayerData();
        playerData.GotHitInGame = false;
        playerData.PlayedGame = false;

        Application.targetFrameRate = 30;
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