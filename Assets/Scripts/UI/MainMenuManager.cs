using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    public TextMeshProUGUI PlayerXPText;
    public TextMeshProUGUI PlayerLevelText;

    [SerializeField] private TextMeshProUGUI version;


    private int levelIndex;
    private string levelName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

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

    private void Start()
    {
        version.text = "ver " + Application.version;
        GameManager.PauseTheGame(false);
        AudioManager.SetMusic("Menu");
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

    public void ShowMessage(string text)
    {
        StartCoroutine(SaveAndExitCoroutine());
        ScreenManager.Instance.ShowMessage(text);
    }

    private IEnumerator SaveAndExitCoroutine()
    {
        yield return new WaitForSeconds(1);
        ScreenManager.Instance.Close();
    }

}