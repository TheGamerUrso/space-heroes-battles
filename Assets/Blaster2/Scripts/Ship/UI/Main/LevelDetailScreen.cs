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

    private Level level;

    public void Setup()
    {
        survivalHighscore.SetActive(false);
    }

    public void ShowDetailScreen()
    {
        level = GameManager.Instance.GetCurrentLevelSelected();

        HideLevelObjectives();

        if (level.mission.ID == (int)LevelEnum.Level0)
        {
            LevelDetailLevelTItle.text = "Survival";
            survivalHighscore.SetActive(true);

            return;
        }

        survivalHighscore.SetActive(false);

        ShowStoryButton.SetActive(true);

        LevelDetailPreview.sprite = sprites[level.mission.SpriteID];
        LevelDetailLevelTItle.text = level.mission.Title;

        levelObjectiveDatas = level.objectiveListData;

        RefreshLevelObjectiveData();
    }

    public void Show(Level level)
    {
        if (level.mission.ID == (int)LevelEnum.Level0)
        {
            ShowStoryButton.SetActive(true);
            LevelDetailLevelTItle.text = "Survival";
        }
        else
        {
            ShowStoryButton.SetActive(true);
            LevelDetailPreview.sprite = sprites[level.spriteId];
            LevelDetailLevelTItle.text = level.mission.Title;
            RefreshLevelObjectiveData();
        }
    }

    public void PlayGame()
    {
        GameManager.Instance.LoadScene((LevelEnum)level.mission.ID);
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
