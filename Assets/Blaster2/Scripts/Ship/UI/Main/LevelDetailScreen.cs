using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelDetailScreen : MonoSingleton<LevelDetailScreen>
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private TextMeshProUGUI LevelDetailLevelTItle;
    [SerializeField] private Image LevelDetailPreview;
    [SerializeField] private GameObject ShowStoryButton;
    [SerializeField] private LevelObjectivesElement[] levelObjectivesElement;
    [SerializeField] private GameObject survivalHighscore;

    private LevelObjectiveData[] levelObjectiveDatas;

    public void Setup()
    {
        survivalHighscore.SetActive(false);
    }

    public void ShowDetailScreen()
    {
        var currentMission = GameManager.Instance.GetCurrentMission();

        HideLevelObjectives();

        if (currentMission.mission.ID == (int)LevelEnum.Level0)
        {
            LevelDetailLevelTItle.text = "Survival";
            survivalHighscore.SetActive(true);
         
            return;
        }

        survivalHighscore.SetActive(false);

        ShowStoryButton.SetActive(true);

        LevelDetailPreview.sprite = sprites[currentMission.mission.SpriteID];
        LevelDetailLevelTItle.text = currentMission.mission.Title;

        levelObjectiveDatas = currentMission.objectiveListData;

        RefreshLevelObjectiveData();
    }

    public void Show(Level level, Mission currentMission)
    {
        if (currentMission.ID == (int)LevelEnum.Level0)
        {
            GameManager.LevelIndexSelected = -1;
            ShowStoryButton.SetActive(true);
            LevelDetailLevelTItle.text = "Survival";
        }
        else
        {
            GameManager.LevelIndexSelected = currentMission.ID;
            ShowStoryButton.SetActive(true);
            LevelDetailPreview.sprite = sprites[level.spriteId];
            LevelDetailLevelTItle.text = currentMission.Title;
            RefreshLevelObjectiveData();
        }
    }

    public void PlayGame()
    {
        var levelIndex = GameManager.LevelIndexSelected;
        GameManager.Instance.LoadScene((LevelEnum)levelIndex);
    }

    public void HideLevelObjectives()
    {
        for (int i = 0; i < levelObjectivesElement.Length; i++)
        {
            levelObjectivesElement[i].gameObject.SetActive(false);
        }
    }

    public void RefreshLevelObjectiveData()
    {
        if (levelObjectiveDatas != null)
        {
            for (int i = 0; i < levelObjectiveDatas.Length; i++)
            {
                if (levelObjectivesElement[i].gameObject.activeSelf == false)
                {
                    levelObjectivesElement[i].gameObject.SetActive(true);
                }
                levelObjectivesElement[i].SetLevelObjective(levelObjectiveDatas[i]);
            }
        }
    }


    public void Close()
    {
        ScreenManager.Instance.Close();
    }
}
