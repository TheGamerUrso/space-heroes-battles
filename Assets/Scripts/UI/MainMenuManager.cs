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

        AudioManager.SetMusic("Menu");

    }



    public void QuitButtonEvent()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        levelIndex = GameManager.LevelSelected;
        levelName = string.Format("Level" + (levelIndex + 1));
        SceneLoader.instance.LoadScene(levelName);
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