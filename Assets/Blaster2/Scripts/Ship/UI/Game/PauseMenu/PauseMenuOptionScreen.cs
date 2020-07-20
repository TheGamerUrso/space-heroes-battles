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
    private MissionCollection listOfMission;
    private Mission mission;
    private LevelObjectiveData[] levelObjectiveDatas;

    public override void InitializeOptions()
    {
        base.InitializeOptions();

        string levelName = "Level" + GameManager.LevelIndexSelected;

        playerData = PersistantData.GetPlayerData();
        listOfMission = PersistantData.GetMissionCollection();
        if (listOfMission != null)
        {
            Scene scene = SceneManager.GetActiveScene();
            string index = scene.name[scene.name.Length - 1].ToString();
            mission = listOfMission.GetMission(int.Parse(index));
            MissionTitle.text = mission.Title;
            levelObjectiveDatas = playerData.GetLevelObjectives(levelName);
            for (int i = 0; i < LevelObjectivesElements.Length; i++)
            {
                LevelObjectivesElements[i].SetLevelObjective(levelObjectiveDatas[i]);
            }
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
        Time.fixedDeltaTime = 0.02f;
        GuiManager.Instance.LoadMainMenu();
    }
}
