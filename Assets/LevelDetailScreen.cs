using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.SceneLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelDetailScreen : Singleton<LevelDetailScreen>
{
    public TextMeshProUGUI LevelDetailLevelTItle;

    public Image LevelDetailPreview;
    public GameObject ShowStoryButton;
    private Mission currentMission;

    public void SetDetails(string Title, Sprite sprite)
    {
        ShowStoryButton.SetActive(true);
        LevelDetailPreview.sprite = sprite;
        LevelDetailLevelTItle.text = Title;
    }

    public void Show(Level level, Mission currentMission)
    {
        if (currentMission.Title.Contains("Survival"))
        {
            GameManager.LevelIndexSelected = -1;
            ShowStoryButton.SetActive(true);
            LevelDetailLevelTItle.text = "Survival";
        }
        else
        {
            GameManager.LevelIndexSelected = currentMission.ID;
            ShowStoryButton.SetActive(true);
            currentMission = GameManager.Instance.GetMission(currentMission.ID);
            LevelDetailPreview.sprite = level.sprite;
            LevelDetailLevelTItle.text = currentMission.Title;
        }
    }


    public void PlayGame()
    {
        var levelIndex = GameManager.LevelIndexSelected;
        var levelName = string.Format("Level" + (levelIndex + 1));
        SceneLoader.Instance.LoadScene(levelName);
    }

    public void Close()
    {
        ScreenManager.Instance.Close();
    }
}
