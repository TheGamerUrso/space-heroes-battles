using System;
using System.Collections;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;

public enum ScreenType
{
    None = 0,
    Challenges = 1,
    Upgrades = 2,
    ShipSelect = 3,
    Options = 4
}

[Serializable]
public class UIScreens
{
    public ScreenType ScreenType;
    public UIView m_UIElement;
    public int test;
}

public class MainMenuManager : MonoBehaviour
{
    public static Action<ScreenType, bool> OnScreenChanged;

    [SerializeField] private TextMeshProUGUI PlayerLevelText;
    [SerializeField] private ShipSelect shipSelect;

    private PlayerShipData playerShipData;
    private PlayerData playerData;

    [SerializeField] private UIScreens[] MainMenuScreens;

    private ScreenType previousScreen;
    private IDataService dataService;
    private IAudioService audioService;
    private IAppService appService;

    [ContextMenu("Get Safe Area")]
    public void GetSafeArea()
    {
        Debug.Log(Screen.safeArea);
    }


    private void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (previousScreen == ScreenType.None)
                {
                    Application.Quit();
                }
                else
                {
                    PlayerData playerData = dataService.GetPlayerData();
                    shipSelect.SelectShip(playerData.CurrrentSelectedShip);

                    Close();
                }
            }
        }
    }

    protected void Awake()
    {
        dataService = GameContext.Get<IDataService>();
        audioService = GameContext.Get<IAudioService>();
        appService = GameContext.Get<IAppService>();
    }

    protected void Start()
    {
        appService.PauseTheGame(false);
        audioService.PlayMusic("Menu");
        Application.targetFrameRate = 30;

        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.ScreenType.Equals(ScreenType.ShipSelect))
            {
                item.m_UIElement.gameObject.SetActive(false);
                OnScreenChanged?.Invoke(item.ScreenType, false);
            }
        }

        playerData = dataService.GetPlayerData();
        playerData.GotHitInGame = false;
        playerData.PlayedGame = false;

        shipSelect.SetShipTexture(playerData.CurrrentSelectedShip);

        shipSelect.OnShipSelected += ShipSelect_OnShipSelected;
    }

    private void ShipSelect_OnShipSelected()
    {
        Close();
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
        LevelEnum[] levels = { LevelEnum.Game };
       SceneLoader.LoadScene(levels[UnityEngine.Random.Range(0, levels.Length)]);

    }

    public GameObject GetUIScreen(ScreenType screenType)
    {
        for (int i = 0; i < MainMenuScreens.Length; i++)
        {
            if (MainMenuScreens[i].ScreenType == screenType)
            {
                return MainMenuScreens[i].m_UIElement.gameObject;
            }
        }
        return null;
    }

    public void OpenMenu()
    {
        StartCoroutine(MenuSwitcher(true));
    }

    public void CloseMenu()
    {
        StartCoroutine(MenuSwitcher(false));
    }

    public bool IsScrene(ScreenType screenType, ScreenType id)
    {
        if (screenType == id)
        {
            return true;
        }
        return false;
    }

    public bool OptionsOrHighscoreOpen()
    {
        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.m_UIElement.IsVisible && item.ScreenType == ScreenType.Options)
            {
                return true;
            }
        }
        return false;
    }

    public void Open(ScreenType Id)
    {
        StartCoroutine(SwitchScreen(Id));
    }

    public void Close()
    {
        if (!OptionsOrHighscoreOpen())
        {
            CloseMenu();
        }
        else
        {
            foreach (UIScreens item in MainMenuScreens)
            {
                if (item.ScreenType == previousScreen)
                {

                    item.m_UIElement.Show();
                    OnScreenChanged?.Invoke(item.ScreenType, true);
                    if (IsScrene(previousScreen, ScreenType.Upgrades))
                    {
                        previousScreen = ScreenType.Challenges;
                    }
                    else
                    {
                        previousScreen = ScreenType.None;
                    }
                }
                else
                {
                    if (item.ScreenType == ScreenType.ShipSelect && item.m_UIElement.IsVisible)
                    {
                        PlayerData playerData = dataService.GetPlayerData();
                        shipSelect.SelectShip(playerData.CurrrentSelectedShip);
                    }

                    item.m_UIElement.Hide();
                    OnScreenChanged?.Invoke(item.ScreenType, false);

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
                previousScreen = item.ScreenType;
            }

            if (OptionsOrHighscoreOpen())
            {
                previousScreen = ScreenType.Challenges;
            }
        }
        yield return null;
    }

    IEnumerator SwitchScreen(ScreenType Id)
    {
        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.ScreenType == Id)
            {
                item.m_UIElement.Show();
                OnScreenChanged?.Invoke(item.ScreenType, true);
            }
            else
            {
                if (item.m_UIElement.IsVisible)
                {
                    if (item.ScreenType != ScreenType.None && item.ScreenType != ScreenType.Options)
                        previousScreen = item.ScreenType;
                }


                if (item.ScreenType != ScreenType.ShipSelect && item.m_UIElement.IsVisible)
                {
                    PlayerData playerData = dataService.GetPlayerData();
                    shipSelect.SelectShip(playerData.CurrrentSelectedShip);
                }

                item.m_UIElement.Hide();
                OnScreenChanged?.Invoke(item.ScreenType, false);
            }
        }
        yield return null;
    }
}