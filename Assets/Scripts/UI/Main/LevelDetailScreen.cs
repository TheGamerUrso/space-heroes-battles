using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelDetailScreen : MonoSingleton<LevelDetailScreen>
{
    public TextMeshProUGUI LevelDetailLevelTItle;

    public Image LevelDetailPreview;
    public GameObject ShowStoryButton;
    private Mission currentMission;
    public LevelObjectivesElement[] levelObjectivesElement;
    private LevelObjectiveData[] levelObjectiveDatas;
    public GameObject survivalHighscore;
    private PlayerData playerData;
    private Dictionary<string, LevelObjectiveData[]> Challanges;

    public void Setup()
    {
        if (playerData == null)
            playerData = PersistantData.GetPlayerData();

        if (Challanges == null)
            Challanges = playerData.GetListOfObjectives();

        survivalHighscore.SetActive(false);
    }

    public void SetDetails(Mission mission, Sprite sprite)
    {
        if (mission == null)
        {
            LevelDetailLevelTItle.text = "Survival";
            survivalHighscore.SetActive(true);
            HideLevelObjectives();
            return;
        }
        survivalHighscore.SetActive(false);
        currentMission = mission;

        ShowStoryButton.SetActive(true);

        LevelDetailPreview.sprite = sprite;
        LevelDetailLevelTItle.text = currentMission.Title;

        levelObjectiveDatas = Challanges["Level" + (currentMission.ID)];

        RefreshLevelObjectiveData();
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
            LevelDetailPreview.sprite = level.sprite;
            LevelDetailLevelTItle.text = currentMission.Title;
            RefreshLevelObjectiveData();
        }
    }


    public void PlayGame()
    {
        var levelIndex = GameManager.LevelIndexSelected;
        var levelName = string.Format("Level" + levelIndex);
        GameManager.Instance.LoadScene(levelName);
    }

    public void Close()
    {
        ScreenManager.Instance.Close();
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
        PlayerData playerData = PersistantData.GetPlayerData();

        if (levelObjectiveDatas != null)
        {
            for (int i = 0; i < levelObjectivesElement.Length; i++)
            {
                if (levelObjectivesElement[i].gameObject.activeSelf == false)
                {
                    levelObjectivesElement[i].gameObject.SetActive(true);
                }
                levelObjectivesElement[i].levelObjectiveData = levelObjectiveDatas[i];
                levelObjectivesElement[i].RefreshLevelObjectiveEement();
            }
        }
    }

}
