using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectScreen : MonoBehaviour
{

    public Sprite[] sprites;

    private PlayerData playerData;
    public GameObject LevelsParentTransform;
    public GameObject LevelElementPrefab;

    public List<GameObject> ListOfLevelElements;

    public float delay;

    [SerializeField] private Mission currentMission;

    public LevelObjectivesElement[] levelObjectivesElement;
    private MissionCollection missionCollection;
    private GameObject LevelElementGO;
    private StoryController dialogueManager;
    private Mission currentMissionSelected;
    private LevelObjectiveData[] levelObjectiveDatas;
    private Dictionary<string, LevelObjectiveData[]> Challanges;
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
        dialogueManager = StoryController.Instance;
        SetLevelSelect();
    }

    public void SetLevelSelect()
    {
        playerData = PersistantData.GetPlayerData();
        missionCollection = PersistantData.GetMissionCollection();
        Challanges = playerData.GetListOfObjectives();

        int missionsCompleted = 1;

        foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
        {
            if (item.Value[0].completed == true)
            {
                missionsCompleted++;
            }
        }

        playerData.LevelUnlocked = missionsCompleted;

        SetLevelSelectButtons();

        LevelDetailScreen.Instance.Setup();
    }

    public void SetLevelSelectButtons()
    {
        LevelElementGO = Instantiate(LevelElementPrefab, LevelsParentTransform.transform, false);

        bool surivalLocked = playerData.SurvivalUnlocked;

        var level = new Level("Survival", new Mission(), null, surivalLocked, surivalLocked);

        var levelElement = LevelElementGO.GetComponent<LevelElement>();

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
        emptyLevelElement.GetComponent<LevelElement>().SetLevelElement(new Level("", new Mission(), null, false, false), null);

    }

    public void StartMissionBriefing(Level level)
    {
        if (level.ID.Contains("Survival"))
        {
            ScreenManager.Instance.Open("LevelDetailScreen");
            GameManager.LevelIndexSelected = 0;
            LevelDetailScreen.Instance.SetDetails(new Mission(), sprites[0]);
        }
        else
        {

            GameManager.LevelIndexSelected = level.mission.ID;
            currentMission = PersistantData.GetMission(level.mission.ID - 1);
            SetMission(currentMission);

            LevelDetailScreen.Instance.SetDetails(currentMission, sprites[currentMission.SpriteID]);

            dialogueManager.ShowStory(GameManager.LevelIndexSelected);
            ScreenManager.Instance.Open("LevelDetailScreen");
        }
    }

    public string GetStory(int missionIndex)
    {
        currentMission = PersistantData.GetMission(missionIndex - 1);
        return currentMission.Description;
    }

    public void SetMission(Mission mission)
    {
        this.currentMission = mission;
    }


    public void Close()
    {
        ScreenManager.Instance.Close();
    }
}
