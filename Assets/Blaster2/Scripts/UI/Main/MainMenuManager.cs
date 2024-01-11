using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using Doozy.Engine.UI;

[Serializable]
public class UIScreens
{
    public string Name;
    public UIView m_UIElement;
    public int test;
}

public class MainMenuManager : MonoSingleton<MainMenuManager>
{
    [SerializeField] private TextMeshProUGUI PlayerLevelText;
    [SerializeField] private ShipSelect shipSelect;

    private PlayerShipData playerShipData;
    private PlayerData playerData;

    [SerializeField] private UIScreens[] MainMenuScreens;

    private string previousScreen;


    [ContextMenu("Get Safe Area")]
    public void GetSafeArea()
    {
        Debug.Log(Screen.safeArea);
    }

    private void Start()
    {
        GameManager.Instance.PauseTheGame(false);
        AudioManager.PlayMusic("Menu");
        Application.targetFrameRate = 30;

        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.Name.Equals("Upgrades"))
            {
                item.m_UIElement.gameObject.SetActive(false);
                Events.OnScreenChanged?.Invoke(item.Name, false);
            }
        }

        playerData = PersistantData.GetPlayerData();
        playerData.GotHitInGame = false;
        playerData.PlayedGame = false;

        shipSelect.SetShipTexture(playerData.CurrrentSelectedShip);
    }


    public void QuitButtonEvent()
    {
#if UNITY_EDITOR_64
        Debug.Log("Quit Game");
        return;
#endif

        GetComponent<CanvasGroup>().blocksRaycasts = false;
        Application.Quit();
    }

    public void PlayGame()
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        // LevelEnum[] levels ={LevelEnum.Level0,LevelEnum.Level1,LevelEnum.Level2,LevelEnum.Level3,LevelEnum.Level4,LevelEnum.Level5,LevelEnum.Level6,LevelEnum.Level7,LevelEnum.Level8,LevelEnum.Level9};
        LevelEnum[] levels = { LevelEnum.Level0 };
        GameManager.Instance.LoadScene(levels[UnityEngine.Random.Range(0, levels.Length)]);

    }

    public GameObject GetUIScreen(string name)
    {
        for (int i = 0; i < MainMenuScreens.Length; i++)
        {
            if (MainMenuScreens[i].Name.Equals("name"))
            {
                return MainMenuScreens[i].m_UIElement.gameObject;
            }
        }
        return null;
    }
    private void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (String.IsNullOrEmpty(previousScreen))
                {
                    Application.Quit();
                }
                else
                {
                    PlayerData playerData = PersistantData.GetPlayerData();
                    ShipSelect.Instance.SelectShip(playerData.CurrrentSelectedShip);

                    Close();
                }
            }
        }
    }

    public void OpenMenu()
    {
        StartCoroutine(MenuSwitcher(true));
    }

    public void CloseMenu()
    {
        StartCoroutine(MenuSwitcher(false));
    }

    public bool IsScrene(string target, string id)
    {
        if (target.Equals(id))
        {
            return true;
        }
        return false;
    }

    public bool OptionsOrHighscoreOpen()
    {
        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.m_UIElement.IsVisible && item.Name.Equals("Options") || item.Name.Equals("HighScore"))
            {
                return true;
            }
        }
        return false;
    }

    public void Open(string Id)
    {
        StartCoroutine(SwitchScreen(Id));
    }

    public void Close()
    {
        if (string.IsNullOrEmpty(previousScreen) || !OptionsOrHighscoreOpen())
        {
            CloseMenu();
        }
        else
        {
            foreach (UIScreens item in MainMenuScreens)
            {
                if (item.Name.Equals(previousScreen))
                {

                    item.m_UIElement.Show();
                    Events.OnScreenChanged?.Invoke(item.Name, true);
                    if (IsScrene(previousScreen, "Upgrades"))
                    {
                        previousScreen = "Quest";
                    }
                    else
                    {
                        previousScreen = "";
                    }
                }
                else
                {
                    if (item.Name.Equals("ShipSelect") && item.m_UIElement.IsVisible)
                    {
                        PlayerData playerData = PersistantData.GetPlayerData();
                        ShipSelect.Instance.SelectShip(playerData.CurrrentSelectedShip);
                    }

                    item.m_UIElement.Hide();
                    Events.OnScreenChanged?.Invoke(item.Name, false);

                }
            }
        }
    }

    IEnumerator MenuSwitcher(bool open)
    {
        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.m_UIElement.IsVisible)
            {
                previousScreen = item.Name;
            }

            if (OptionsOrHighscoreOpen())
            {
                previousScreen = "Quest";
            }
        }
        yield return null;
    }

    IEnumerator SwitchScreen(string Id)
    {
        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.Name.Equals(Id))
            {
                item.m_UIElement.Show();
                Events.OnScreenChanged?.Invoke(item.Name, true);
            }
            else
            {
                if (item.m_UIElement.IsVisible)
                {
                    if (!item.Name.Equals("Menu") && !item.Name.Equals("Options") && !item.Name.Equals("HighScore"))
                        previousScreen = item.Name;
                }

                if (item.Name.Equals("ShipSelect") && item.m_UIElement.IsVisible)
                {
                    PlayerData playerData = PersistantData.GetPlayerData();
                    ShipSelect.Instance.SelectShip(playerData.CurrrentSelectedShip);
                }

                item.m_UIElement.Hide();
                Events.OnScreenChanged?.Invoke(item.Name, false);
            }
        }
        yield return null;
    }
}