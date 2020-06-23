using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

    [SerializeField] private Dictionary<string, ObjectiveData> ListOfObjectives = new Dictionary<string, ObjectiveData>();
    [SerializeField] private GameObject[] ObjectiveLocations;
    private List<ObjectiveType> ListOfAvailableObjectiveTypes;
    private float ResetTimer = 2f;
    private bool allObjectivesCompleted = false;
    private int objectiveIndex = 0;


    private void Start()
    {
        InitializeObjectives();
        RefreshObjectives();
    }

    public void CreateNewObjective()
    {
        ListOfAvailableObjectiveTypes = Enum.GetValues(typeof(ObjectiveType)).Cast<ObjectiveType>().ToList();
        PlayerData playerData = PersistantData.GetPlayerData();

        ObjectiveData objectiveData = null;
        ObjectiveType objectiveType;

        for (int i = 0; i < 3; i++)
        {
            int randoNumber = UnityEngine.Random.Range(0, ListOfAvailableObjectiveTypes.Count);

            objectiveData = null;
            objectiveType = ListOfAvailableObjectiveTypes[randoNumber];


            switch (objectiveType)
            {
                case ObjectiveType.Kill:
                    objectiveData = new ObjectiveData(i, "Defeat", UnityEngine.Random.Range(100, 500), 0, 0, "Kill <color=orange> X / % </color>   enemies");
                    break;

                case ObjectiveType.Use:
                    objectiveData = new ObjectiveData(i, "Use", UnityEngine.Random.Range(10, 25), 0, 1, "Use super <color=orange> X / % </color>  times");
                    break;

                case ObjectiveType.Unharmed:
                    objectiveData = new ObjectiveData(i, "Unharmed", 1, 0, 2, "Complete a level without getting hit");
                    break;

                case ObjectiveType.survive:
                    objectiveData = new ObjectiveData(i, "Survie", UnityEngine.Random.Range(1, 7), 0, 3, "Play <color=orange> X / % </color>  Levels");
                    break;

                case ObjectiveType.spend:
                    objectiveData = new ObjectiveData(i, "Spend_1", UnityEngine.Random.Range(500, 1000), 0, 4, "Spend <color=orange> X / % </color>  coins");
                    break;

                default:
                    break;
            }

            ListOfAvailableObjectiveTypes.Remove(objectiveType);

            ListOfObjectives.Add(objectiveData.Id, objectiveData);
        }


        GameObject objectiveGO;
        ObjectivesElement objectivesElement;
        foreach (ObjectiveData item in ListOfObjectives.Values)
        {
            objectiveGO = ObjectiveLocations[objectiveIndex];
            objectiveGO.gameObject.SetActive(true);

            objectivesElement = objectiveGO.GetComponent<ObjectivesElement>();
            objectivesElement.InitializeObjective(item);
            Events.OnObjectiveChange += CheckObjective;
            objectivesElement.ResetStatus();

            if (objectiveGO.GetComponent<ObjectivesElement>().objectiveData != null)
            {
                playerData.ListOfOnGoingObjectives.Add(objectiveGO.GetComponent<ObjectivesElement>().objectiveData);
                objectiveIndex++;
            }
        }

        RefreshObjectives();
    }

    public void CheckObjective(object sender, Events.ObjectiveEventArgs e)
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        ObjectiveData objectiveData = e.objectiveData;

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
        PlayerData playerData = PersistantData.GetPlayerData();

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
                Events.OnObjectiveChange += CheckObjective;

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