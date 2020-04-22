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

public class ScreenManager : MonoBehaviour
{
    public Action<string, bool> OnScreenChanged;
    public static ScreenManager Instance;
    private AudioManager AudioAPI;
    public UIScreens[] MainMenuScreens;
    private string previousScreen;

    private void Awake()
    {
        Instance = this;
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
    private void Start()
    {
        AudioAPI = AudioManager.Instance;
        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.Name.Equals("Upgrades") || item.Name.Equals("Levels"))
            {
                item.m_UIElement.gameObject.SetActive(false);
                OnScreenChanged?.Invoke(item.Name, false);
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
                    if (DialogueManager.Instance.StoryWindowIsOpen())
                    {
                        DialogueManager.Instance.Close();
                    }
                    else if (DialogueManager.Instance.StoryWindowIsOpen() == false)
                    {
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



    IEnumerator MenuSwitcher(bool open)
    {
        //AudioManager.PlaySound(null,"Click", 1);

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

            //if (item.Name.Equals("Menu"))
            //{
            //    if (open)
            //    {
            //        item.m_UIElement.Show();
            //        OnScreenChanged?.Invoke(item.Name, true);
            //    }
            //    else
            //    {
            //        item.m_UIElement.Hide();
            //        OnScreenChanged?.Invoke(item.Name, false);
            //    }
            //}
        }
        yield return null;
    }

    public void CloseMenu()
    {
        //AudioManager.instance.PlaySound("Back", 1);
        //foreach (UIScreens item in MainMenuScreens)
        //{
        //    if (OptionsOrHighscoreOpen())
        //    {
        //        previousScreen = "Quest";
        //    }

        //    if (item.Name.Equals("Menu"))
        //    {
        //        item.m_UIElement.SetActive(false);
        //    }
        //}


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

    public void Close()
    {
        //AudioManager.PlaySound(null, "Back", 1);
        // if (string.IsNullOrEmpty(previousScreen) || previousScreen.Equals("Menu") || !OptionsOrHighscoreOpen())
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
                    OnScreenChanged?.Invoke(item.Name, true);
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
                    item.m_UIElement.Hide();
                    OnScreenChanged?.Invoke(item.Name, false);
                }
            }
        }
    }


    IEnumerator SwitchScreen(string Id)
    {
        //AudioManager.PlaySound(null, "Click", 1);
        // if (Id.Equals("Menu"))
        //  {
        //      OpenMenu();
        //  }
        //  else
        //  {
        foreach (UIScreens item in MainMenuScreens)
        {
            if (item.Name.Equals(Id))
            {
                item.m_UIElement.Show();
                OnScreenChanged?.Invoke(item.Name, true);
            }
            else
            {
                if (item.m_UIElement.IsVisible)
                {
                    if (!item.Name.Equals("Menu") && !item.Name.Equals("Options") && !item.Name.Equals("HighScore"))
                        previousScreen = item.Name;
                }

                item.m_UIElement.Hide();
                OnScreenChanged?.Invoke(item.Name, false);
            }
        }
        //  }
        yield return null;

    }
    public void Open(string Id)
    {
        //AudioManager.instance.PlaySound("Click", 1);
        //if (Id.Equals("Menu"))
        //{
        //    OpenMenu();
        //}
        //else
        //{
        //    foreach (UIScreens item in MainMenuScreens)
        //    {
        //        if (item.Name.Equals(Id))
        //        {
        //            item.m_UIElement.SetActive(true);
        //        }
        //        else
        //        {
        //            if (item.m_UIElement.activeSelf)
        //            {
        //                if (!item.Name.Equals("Menu") && !item.Name.Equals("Options") && !item.Name.Equals("HighScore"))
        //                    previousScreen = item.Name;
        //            }
        //            item.m_UIElement.SetActive(false);
        //        }
        //    }
        //}

        StartCoroutine(SwitchScreen(Id));
    }
}