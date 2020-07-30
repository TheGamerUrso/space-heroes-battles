using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

    [SerializeField] private Dictionary<string, ObjectiveData> ListOfObjectives = new Dictionary<string, ObjectiveData>();
    [SerializeField] private GameObject[] ObjectiveLocations;
    private List<ObjectiveTypeEnum> ListOfAvailableObjectiveTypes;
    private float ResetTimer = 2f;
    private bool allObjectivesCompleted = false;

    private PlayerData playerData;

    private void Start()
    {
        playerData = PersistantData.GetPlayerData();

        InitializeObjectives();
        RefreshObjectives();

        Events.OnObjectiveChange += CheckObjective;
    }

    public void CreateNewObjective()
    {
        ListOfAvailableObjectiveTypes = Enum.GetValues(typeof(ObjectiveTypeEnum)).Cast<ObjectiveTypeEnum>().ToList();

        ObjectiveTypeEnum objectiveType;
        ObjectiveData objectiveData = null;

        for (int i = 0; i < 3; i++)
        {
            int randoNumber = UnityEngine.Random.Range(0, ListOfAvailableObjectiveTypes.Count);

            objectiveType = ListOfAvailableObjectiveTypes[randoNumber];

            switch (objectiveType)
            {
                case ObjectiveTypeEnum.KILL:
                    objectiveData = new ObjectiveData(i, "Defeat", UnityEngine.Random.Range(100, 500), 0, (int)objectiveType, "Kill <color=orange> X / % </color>   enemies");
                    break;
                case ObjectiveTypeEnum.USE:
                    objectiveData = new ObjectiveData(i, "Use", UnityEngine.Random.Range(10, 25), 0, (int)objectiveType, "Use super <color=orange> X / % </color>  times");
                    break;
                case ObjectiveTypeEnum.UNHARMED:
                    objectiveData = new ObjectiveData(i, "Unharmed", 1, 0, (int)objectiveType, "Complete a level without getting hit");
                    break;
                case ObjectiveTypeEnum.SURVIVE:
                    objectiveData = new ObjectiveData(i, "Survie", UnityEngine.Random.Range(1, 7), 0, (int)objectiveType, "Play Level <color=orange> X  </color>");
                    break;
                case ObjectiveTypeEnum.SPEND:
                    objectiveData = new ObjectiveData(i, "Spend_1", UnityEngine.Random.Range(100, 250), 0, (int)objectiveType, "Spend <color=orange> X / % </color>  coins");
                    break;
                case ObjectiveTypeEnum.BOUNTY:
                    objectiveData = new ObjectiveData(i, "BOUNTY", UnityEngine.Random.Range(1, 5), 0, (int)objectiveType, "Kill BOSS <color=orange> X  </color>");
                    break;
                case ObjectiveTypeEnum.SCORE:
                    objectiveData = new ObjectiveData(i, "SCORE", UnityEngine.Random.Range(10000, 50000), 0, (int)objectiveType, "Achieve <color=orange> X  </color> Score In a Level");
                    break;
            }

            ListOfAvailableObjectiveTypes.Remove(objectiveType);

            ListOfObjectives.Add(objectiveData.Id, objectiveData);
        }


        var objectiveIndex = 0;
        GameObject objectiveGO;
        ObjectivesElement objectivesElement;

        foreach (ObjectiveData item in ListOfObjectives.Values)
        {
            objectiveGO = ObjectiveLocations[objectiveIndex];
            objectiveGO.gameObject.SetActive(true);

            objectivesElement = objectiveGO.GetComponent<ObjectivesElement>();
            objectivesElement.InitializeObjective(item);

            objectivesElement.ResetStatus();

            if (objectiveGO.GetComponent<ObjectivesElement>().objectiveData != null)
            {
                playerData.ListOfOnGoingObjectives.Add(objectiveGO.GetComponent<ObjectivesElement>().objectiveData);
                objectiveIndex++;
            }
        }

        RefreshObjectives();
    }

    public void CheckObjective(ObjectiveData objectiveData)
    {
        ListOfObjectives.Remove(objectiveData.Id);

        int numberOfCompletdQuest = 0;

        foreach (var item in playerData.ListOfOnGoingObjectives.ToList())
        {
            if (item.claimed)
            {
                numberOfCompletdQuest++;
            }
        }

        if (numberOfCompletdQuest == 3)
        {
            LoadingNewObjectives();
            allObjectivesCompleted = true;
        }
    }

    private void Update()
    {
        if (ResetTimer > 0 && allObjectivesCompleted)
        {
            ResetTimer -= Time.deltaTime;
        }
        else if (ResetTimer <= 0 && allObjectivesCompleted)
        {
            allObjectivesCompleted = false;
            ResetTimer = 2f;
            GenerateNewObjectives();
        }
        else
        {
            RefreshObjectives();
        }

    }

    public void LoadingNewObjectives()
    {
        GameObject objectiveGO;
        for (int i = 0; i < ObjectiveLocations.Length; i++)
        {
            objectiveGO = ObjectiveLocations[i];
            objectiveGO.GetComponent<ObjectivesElement>().LoadingIndicator();
        }
    }

    public void GenerateNewObjectives()
    {
        foreach (var item in playerData.ListOfOnGoingObjectives.ToList())
        {
            playerData.ListOfOnGoingObjectives.Remove(item);
            InitializeObjectives();
            RefreshObjectives();
        }
    }

    public void RefreshObjectives()
    {
        GameObject objectiveGO;
        for (int i = 0; i < ObjectiveLocations.Length; i++)
        {
            objectiveGO = ObjectiveLocations[i];
            objectiveGO.GetComponent<ObjectivesElement>().RefreshQuests();
        }
    }

    public void InitializeObjectives()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        if (playerData.ListOfOnGoingObjectives.Count > 0)
        {
            var objectiveIndex = 0;
            GameObject objectiveGO;
            ObjectivesElement objectivesElement;
            foreach (ObjectiveData item in playerData.ListOfOnGoingObjectives.ToList())
            {
                objectiveGO = ObjectiveLocations[objectiveIndex];
                objectivesElement = objectiveGO.GetComponent<ObjectivesElement>();
                objectivesElement.InitializeObjective(item);
                objectiveIndex++;
            }
        }
        else { CreateNewObjective(); }
    }

    public void PlayButton()
    {
        ScreenManager.Instance.Open("Levels");
    }
}