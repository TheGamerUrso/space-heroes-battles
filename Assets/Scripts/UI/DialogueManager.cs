using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public string name;
    public int user;

    [TextArea(3, 10)]
    public string text;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public ConversationWidget ConversationWidget;
    private int currentMission;

    private void Awake()
    {
        Instance = this;
    }

    public void Open()
    {
        Mission mission = DataController.Instance.GetMission(currentMission);
        ConversationWidget.SetStory(mission.Description);
        ConversationWidget.gameObject.SetActive(true);


    }

    public void Close()
    {
        ConversationWidget.gameObject.SetActive(false);
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