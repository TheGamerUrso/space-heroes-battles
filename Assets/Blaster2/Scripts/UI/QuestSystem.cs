using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestSystem : MonoSingleton<QuestSystem>
{
    public Action LoadingNewQuests;
    public Action OnNewQuestGenerated;
    public Action OnQuestValueChanged;
    [SerializeField] private Dictionary<string, ObjectiveData> ListOfObjectives = new Dictionary<string, ObjectiveData>();
    //[SerializeField] private GameObject[] ObjectiveLocations;
    [SerializeField] private List<ObjectiveTypeEnum> ListOfAvailableObjectiveTypes;
    private float ResetTimer = 2f;
    private bool allObjectivesCompleted = false;

    private PlayerData playerData;
    public List<ObjectiveData> ListOfOnGoingObjectives = new List<ObjectiveData>();
    public List<ObjectiveData> ListOfActiveQuest{get{return ListOfOnGoingObjectives;}}
    private void OnEnable()
    {
        OnQuestValueChanged?.Invoke();
    }

    private void Start()
    {
        playerData = PersistantData.GetPlayerData();
        ListOfOnGoingObjectives = playerData.ListOfOnGoingObjectives;
        InitializeObjectives();
    }

    public void InitializeObjectives()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        if (playerData.ListOfOnGoingObjectives.Count > 0)
        {
            OnNewQuestGenerated?.Invoke();
        }
        else
        {
            CreateNewObjective();
        }
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
                    objectiveData = new ObjectiveData(i, "DEFEAT ENEMIES", UnityEngine.Random.Range(128, 256), 0, (int)objectiveType, "Kill <color=orange> X / % </color>   enemies");
                    break;
                case ObjectiveTypeEnum.USE:
                    objectiveData = new ObjectiveData(i, "SPECIAL ATTACK USED", UnityEngine.Random.Range(3, 10), 0, (int)objectiveType, "Use super <color=orange> X / % </color>  times");
                    break;
                case ObjectiveTypeEnum.UNHARMED:
                    objectiveData = new ObjectiveData(i, "UNHARMED", UnityEngine.Random.Range(10000, 50000), 0, (int)objectiveType, "Achieve <color=orange> % </color> Score Without Getting Hit");
                    break;
                case ObjectiveTypeEnum.SURVIVE:
                    objectiveData = new ObjectiveData(i, "SURVIVE", UnityEngine.Random.Range(8, 25), 0, (int)objectiveType, "Survive <color=orange> % </color> Waves");
                    break;
                case ObjectiveTypeEnum.SPEND:
                    objectiveData = new ObjectiveData(i, "SPEND", UnityEngine.Random.Range(100, 250), 0, (int)objectiveType, "Spend <color=orange> X / % </color> coins");
                    break;
                case ObjectiveTypeEnum.BOUNTY:
                    objectiveData = new ObjectiveData(i, "BOUNTY", UnityEngine.Random.Range(0, 4), 0, (int)objectiveType, "Kill  <color=orange> % </color> BOSS");
                    break;
                case ObjectiveTypeEnum.SCORE:
                    objectiveData = new ObjectiveData(i, "SCORE", UnityEngine.Random.Range(10000, 50000), 0, (int)objectiveType, "Achieve <color=orange> % </color> Total Score");
                    break;
            }

            ListOfAvailableObjectiveTypes.Remove(objectiveType);

            ListOfObjectives.Add(objectiveData.Id, objectiveData);
        }


        var objectiveIndex = 0;

        foreach (ObjectiveData item in ListOfObjectives.Values)
        {
            if (item != null)
            {
                playerData.ListOfOnGoingObjectives.Add(item);
                objectiveIndex++;
            }
        }

        OnNewQuestGenerated?.Invoke();
    }
    
    public void CompleteQuest(ObjectiveData objectiveData)
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
            LoadingNewQuests?.Invoke();
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
    }


    public void GenerateNewObjectives()
    {
        foreach (var item in playerData.ListOfOnGoingObjectives.ToList())
        {
            playerData.ListOfOnGoingObjectives.Remove(item);
            InitializeObjectives();
        }
    }


    [ContextMenu("Generate New Challenges")]
    public void GeneratedQuest()
    {
        GenerateNewObjectives();
    }

    public void SetQuestProgressByType(ObjectiveTypeEnum type, int progress)
    {
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(type);
        if (objectiveData != null)
        {
            if (type == ObjectiveTypeEnum.SURVIVE)
            {
                if (playerData.GotHitInGame) return;
            }
            objectiveData.UpdateProgress(progress);
        }
        OnQuestValueChanged?.Invoke();
    }

    [ContextMenu("Finish Quests")]
    public void FinishQuest()
    {
        for (int i = 0; i < playerData.ListOfOnGoingObjectives.Count; i++)
        {
            ObjectiveData objective = playerData.ListOfOnGoingObjectives[i];
            objective.UpdateProgress(objective.requirment);
        }
        OnQuestValueChanged?.Invoke();
    }
}