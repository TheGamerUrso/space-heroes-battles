using Doozy.Engine.UI;
using System;
using UnityEngine;

public class StoryController : MonoSingleton<StoryController>
{
    public GameObject ConversationWidget;
    private int currentMission;

    public void Open()
    {
   
        Mission mission = PersistantData.GetMission(currentMission - 1);
        ConversationWidget.GetComponent<StoryView>().SetStory(mission.Description);

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

    public void ShowStory(int missionIndex)
    {
        currentMission = missionIndex;
        if (!PlayerPrefsUtils.LoadBool("Level" + currentMission))
        {
            Open();
            PlayerPrefsUtils.SaveBool("Level" + currentMission, 1);
        }
    }
}