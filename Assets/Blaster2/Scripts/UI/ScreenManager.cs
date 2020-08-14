using System;
using System.Collections;
using UnityEngine;
using Doozy.Engine.UI;

[Serializable]
public class UIScreens
{
    public string Name;
    public UIView m_UIElement;
}

public class ScreenManager : MonoSingleton<ScreenManager>
{
    [SerializeField] private UIScreens[] MainMenuScreens;

    private string previousScreen;

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

    private void Start()
    {
        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.Name.Equals("Upgrades") || item.Name.Equals("Levels"))
            {
                item.m_UIElement.gameObject.SetActive(false);
                Events.OnScreenChanged?.Invoke(item.Name, false);
            }
        }
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
                    if (StoryController.Instance.StoryWindowIsOpen())
                    {
                        StoryController.Instance.Close();
                    }
                    else if (StoryController.Instance.StoryWindowIsOpen() == false)
                    {
                        PlayerData playerData = PersistantData.GetPlayerData();
                        ShipSelect.Instance.SelectShip(playerData.CurrrentSelectedShip);

                        Close();
                    }
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
                    if (IsScrene(previousScreen, "Levels") || IsScrene(previousScreen, "Upgrades"))
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