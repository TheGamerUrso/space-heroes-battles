using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuOptionScreen : BaseOptions
{
    public TextMeshProUGUI MissionTitle;
    public LevelObjectivesElement[] LevelObjectivesElements;
    private PlayerData playerData;
    private Level currentLevel;
    private Mission mission;
    private LevelObjectiveData[] levelObjectiveDatas;

    public override void InitializeOptions()
    {
        base.InitializeOptions();
        Scene scene = SceneManager.GetActiveScene();

        string levelName = ((LevelEnum)scene.buildIndex).ToString();

        playerData = PersistantData.GetPlayerData();

        int levelMission = scene.buildIndex - (int)LevelEnum.Level0;
        currentLevel = PersistantData.GetLevels()[levelMission];

        mission = currentLevel.mission;

        MissionTitle.text = mission.Title;

        levelObjectiveDatas = currentLevel.objectiveListData;

        for (int i = 0; i < LevelObjectivesElements.Length; i++)
        {
            LevelObjectivesElements[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < levelObjectiveDatas.Length; i++)
        {
            LevelObjectivesElements[i].SetLevelObjective(levelObjectiveDatas[i]);
            LevelObjectivesElements[i].gameObject.SetActive(true);
        }

    }

    public override void OnOptionEnter()
    {
        base.OnOptionEnter();

        RefreshLevelObjectiveElements();
    }

    public void RefreshLevelObjectiveElements()
    {
        if (levelObjectiveDatas == null)
        {
            return;
        }

        for (int i = 0; i < levelObjectiveDatas.Length; i++)
        {
            if (levelObjectiveDatas[i].completed)
            {
                LevelObjectivesElements[i].CheckComplete();
            }
        }
    }

    public override void ExitAndSave()
    {
        base.ExitAndSave();
        GuiManager.Instance.ResumeButton();
    }

    public void Quit()
    {
        base.ExitAndSave();
        Time.fixedDeltaTime = 0.02f;
        GuiManager.Instance.LoadMainMenu();
    }
}
