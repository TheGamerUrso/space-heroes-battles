using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenuOptionScreen : BaseOptions
{
    public TextMeshProUGUI MissionTitle;
    public LevelObjectivesElement[] LevelObjectivesElements;
    private PlayerData playerData;
    private MissionCollection listOfMission;
    private Mission mission;
    private LevelObjectiveData[] levelObjectiveDatas;

    public override void InitializeOptions()
    {
        base.InitializeOptions();
        string levelName = "Level" + GameManager.LevelSelected;

        playerData = DataController.GetPlayerData();
        listOfMission = DataController.GetMissionCollection();
        mission = listOfMission.GetMission(GameManager.LevelSelected);
        MissionTitle.text = mission.Title;
        levelObjectiveDatas = playerData.GetLevelObjectives(levelName);
        for (int i = 0; i < LevelObjectivesElements.Length; i++)
        {
            LevelObjectivesElements[i].levelObjectiveData = levelObjectiveDatas[i];
        }
    }

    public override void OnOptionEnter()
    {
        base.OnOptionEnter();
        RefreshLevelObjectiveElements();
    }

    public void RefreshLevelObjectiveElements()
    {   
        for (int i = 0; i < levelObjectiveDatas.Length; i++)
        {
            if (levelObjectiveDatas[i].completed)
            {
                LevelObjectivesElements[i].RefreshLevelObjectiveEement();

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
        GuiManager.Instance.LoadMainMenu();
    }
}
