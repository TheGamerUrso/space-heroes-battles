using Doozy.Engine.UI;
using System;
using UnityEngine;

public class StoryController : MonoSingleton<StoryController>
{
    public GameObject ConversationWidget;
    private Level level;

    public void Open()
    {
        level = GameManager.Instance.GetCurrentLevelSelected();

        ConversationWidget.GetComponent<StoryView>().SetStory(level.mission.Description);

        ConversationWidget.SetActive(true);
    }

    public void Close()
    {
        ConversationWidget.SetActive(false);
    }

    public bool StoryWindowIsOpen()
    {
        if (ConversationWidget.gameObject.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
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
                Close();
            }
        }
    }

    public void ShowStory(Level level)
    {
        this.level = level;
        if (!PlayerPrefsUtils.LoadBool(level.ID))
        {
            Open();
            PlayerPrefsUtils.SaveBool(level.ID, 1);
        }
    }
}