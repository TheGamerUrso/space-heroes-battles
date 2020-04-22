using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BriefingScreen : MonoBehaviour
{
    public TextMeshProUGUI LevelDetailLevelTItle;
    public Sprite[] sprites;
    public Image LevelDetailPreview;

    public GameObject LevelsParentTransform;
    public GameObject LevelElementPrefab;

    public List<GameObject> ListOfLevelElements;

    public float delay;

    [SerializeField] private Mission currentMission;

    public LevelObjectivesElement[] levelObjectivesElement;
    private MissionCollection missionCollection;
    private GameObject LevelElementGO;
    private DialogueManager dialogueManager;
    private Mission currentMissionSelected;
    private LevelObjectiveData[] levelObjectiveDatas;

    public GameObject ShowStoryButton;
    public GameObject announcementMessage;

    public void RefreshLevelElementByID(int CompleteLevelIndex)
    {
        ListOfLevelElements[CompleteLevelIndex].GetComponent<LevelElement>().Refresh();

    }
    public void RefreshLevelElements()
    {
        for (int i = 0; i < ListOfLevelElements.Count; i++)
        {
            GameObject levelElement = ListOfLevelElements[i];

            levelElement.GetComponent<LevelElement>().Refresh();
        }
    }

    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
        InitizeBriefingScreen();
    }

    public void InitizeBriefingScreen()
    {
        MissionBriefingInit();

        RefreshLevelObjectiveData();

        string title = "";

        if (currentMission != null)
        {
            //gameObject.name = string.Format("ID {0} - Mission: {1}", mission.ID, mission.Title);
            title = currentMission.Title;
        }

        SetMissionTitle(title);
    }

    public void MissionBriefingInit()
    {
        missionCollection = PersistantData.GetMissionCollection();
        Button missionButton;
        LevelElement levelElement;
        Level level;
        Image LevelImage;

        PlayerData playerData = PersistantData.GetPlayerData();
        Dictionary<string, LevelObjectiveData[]> Challanges = playerData.GetListOfObjectives();
        int missionsCompleted = 0;
       
        foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
        {
            if (item.Value[0].completed == true)
            {
                missionsCompleted++;
            }
        }

        playerData.LevelUnlocked = missionsCompleted;

        LevelElementGO = Instantiate(LevelElementPrefab, LevelsParentTransform.transform, false);

        level = new Level("Survival", null,null, true, false);

        levelElement = LevelElementGO.GetComponent<LevelElement>();

        levelElement.SetLevelElement(level, StartMissionBriefing);

        ListOfLevelElements.Add(LevelElementGO);

        for (int i = 0; i < missionCollection.Missions.Length; i++)
        {
            Mission missionItem = missionCollection.Missions[i];
            LevelElementGO = Instantiate(LevelElementPrefab, LevelsParentTransform.transform, false);

            level = new Level(missionItem.Title, missionItem, sprites[missionItem.SpriteID], false, true);

            if (missionItem.ID <= playerData.LevelUnlocked)
            {
                level.interactable = true;
                level.Locked = false;
            }

            levelElement = LevelElementGO.GetComponent<LevelElement>();

            levelElement.SetLevelElement(level, StartMissionBriefing);

            ListOfLevelElements.Add(LevelElementGO);
        }

        GameObject emptyLevelElement = Instantiate(LevelElementPrefab, LevelsParentTransform.transform, false);
        emptyLevelElement.GetComponent<LevelElement>().SetLevelElement(new Level("", null, null, false, false), null);
    }

    public void StartMissionBriefing(Level level)
    {
        if (level.ID.Contains("Survival"))
        {
            ScreenManager.Instance.Open("Briefing");
            GameManager.LevelIndexSelected = -1;
            RefreshLevelObjectiveData();
            ShowStoryButton.SetActive(true);
            LevelDetailLevelTItle.text = "Survival";
        }
        else
        {
            ScreenManager.Instance.Open("Briefing");
            GameManager.LevelIndexSelected = level.mission.ID;

            RefreshLevelObjectiveData();
            ShowStoryButton.SetActive(true);
            currentMission = PersistantData.GetMission(level.mission.ID);
            LevelDetailPreview.sprite = sprites[currentMission.SpriteID];
            LevelDetailLevelTItle.text = currentMission.Title;
            SetMission(currentMission);
            dialogueManager.ShowStory(GameManager.LevelIndexSelected);
        }
    }

    public string GetStory(int missionIndex)
    {
        currentMission = PersistantData.GetMission(missionIndex);
        return currentMission.Description;
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
        levelObjectiveDatas = playerData.GetLevelObjectivesByID("Level" + GameManager.LevelIndexSelected);
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

    public void SetMission(Mission mission)
    {
        this.currentMission = mission;
    }

    public void SetMissionTitle(string title)
    {
        LevelDetailLevelTItle.text = title;
    }
}