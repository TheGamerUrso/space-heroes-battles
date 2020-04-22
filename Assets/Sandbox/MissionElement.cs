using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionElement : MonoBehaviour {

    [SerializeField] private Mission mission;
    [SerializeField] private TextMeshProUGUI MissionTitle = null;
    [SerializeField] private TextMeshProUGUI StoryText;
    [SerializeField] private bool WaitForInput;
    [SerializeField] private string story;

    public TextMeshProUGUI Title;
    public Sprite[] sprites;
    public Image MissionImage;

    public void SetMission(Mission mission)
    {
        this.mission = mission;
    }


    private void OnEnable()
    {
        string title = "";
        story = "";
        if (mission  != null)
        {
            gameObject.name = string.Format("ID {0} - Mission: {1}", mission.ID, mission.Title);
            title = mission.Title;
            story = mission.Description;

            MissionImage.sprite = sprites[mission.ID];
            Title.text = mission.Title;

        }
        SetMissionTitle(title);
        SetStoryText(story);

        StartCoroutine(PlayStory());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && WaitForInput)
        {
            SetStoryText(story);
            WaitForInput = false;
        }
    }

    IEnumerator PlayStory()
    {
        WaitForInput = true;
        SetStoryText("");
        var tempStory = "";

        foreach (char letter in story.ToCharArray())
        {
            if (WaitForInput == false)
            {
                break;
            }
            tempStory += letter;
            SetStoryText(tempStory);
            yield return null;
        }

        SetStoryText(story);
    }

    public void SetMissionTitle(string title)
    {
        MissionTitle.text = title;
    }

    public void SetStoryText(string descriptiopn)
    {
        StoryText.text = descriptiopn;
    }
}
