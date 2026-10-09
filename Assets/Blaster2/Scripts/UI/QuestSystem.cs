using System;
using System.Collections.Generic;
using System.Linq;
using TheGamerUrso.Core;
using Unity.ProjectAuditor.Editor;
using UnityEngine;
using UnityEngine.Playables;

[DefaultExecutionOrder(-100)]
public class QuestSystem : ServiceComponent<IQuestService>, IQuestService
{
    public event Action LoadingNewQuests;
    public event Action OnNewQuestGenerated;
    public event Action OnQuestValueChanged;
    [SerializeField] private Dictionary<string, QuestData> DictOfActiveQuests = new Dictionary<string, QuestData>();

    [SerializeField] private List<QuestTypeEnum> ListOfAvailableQuestType;
    private float ResetTimer = 2f;
    private bool AllQuestsCompleted = false;
    public List<QuestData> ActiveQuests = new List<QuestData>();
    public List<QuestData> ListOfActiveQuest { get { return ActiveQuests; } }

    private INotificationService notificationService;
    private IQuestService questService;
    private IEventService eventService;

    private void OnEnable()
    {
  
    }

    private void Start()
    {
        InitializeQuests();
        eventService = GameContext.Get<IEventService>();
        notificationService = GameContext.Get<INotificationService>();

        eventService.Subscribe<QuestProgressEvent>(OnQuestProgressHandled);
        OnQuestValueChanged?.Invoke();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        eventService.Unsubscribe<QuestProgressEvent>(OnQuestProgressHandled);
    }
    public void InitializeQuests()
    {

        CreateNewQuests();
    }

    public void CreateNewQuests()
    {
        ListOfAvailableQuestType = Enum.GetValues(typeof(QuestTypeEnum)).Cast<QuestTypeEnum>().ToList();

        QuestTypeEnum questType;
        QuestData questData = null;

        for (int i = 0; i < 3; i++)
        {
            int randoNumber = UnityEngine.Random.Range(0, ListOfAvailableQuestType.Count);

            questType = ListOfAvailableQuestType[randoNumber];

            switch (questType)
            {
                case QuestTypeEnum.KILL:
                    questData = new QuestData(i, "DEFEAT ENEMIES", UnityEngine.Random.Range(200, 500), 0, (int)questType, "Kill <color=orange> X / % </color>   enemies");
                    break;
                case QuestTypeEnum.USE:
                    questData = new QuestData(i, "SPECIAL ATTACK USED", UnityEngine.Random.Range(3, 10), 0, (int)questType, "Use super <color=orange> X / % </color>  times");
                    break;
                case QuestTypeEnum.UNHARMED:
                    questData = new QuestData(i, "UNHARMED", UnityEngine.Random.Range(10000, 50000), 0, (int)questType, "Achieve <color=orange> % </color> Score Without Getting Hit");
                    break;
                case QuestTypeEnum.SURVIVE:
                    questData = new QuestData(i, "SURVIVE", UnityEngine.Random.Range(10, 50), 0, (int)questType, "Survive <color=orange> % </color> Waves");
                    break;
                case QuestTypeEnum.SPEND:
                    questData = new QuestData(i, "SPEND", UnityEngine.Random.Range(500, 1000), 0, (int)questType, "Spend <color=orange> X / % </color> coins");
                    break;
                case QuestTypeEnum.BOUNTY:
                    questData = new QuestData(i, "BOUNTY", 1, 0, (int)questType, "Defeat a strong Enemy");
                    break;
                case QuestTypeEnum.SCORE:
                    questData = new QuestData(i, "SCORE", UnityEngine.Random.Range(10000, 50000), 0, (int)questType, "Achieve <color=orange> % </color> Total Score");
                    break;
            }

            ListOfAvailableQuestType.Remove(questType);

            DictOfActiveQuests.Add(questData.Id, questData);
        }


        var objectiveIndex = 0;

        foreach (QuestData item in DictOfActiveQuests.Values)
        {
            if (item != null)
            {
                ActiveQuests.Add(item);
                objectiveIndex++;
            }
        }

        OnNewQuestGenerated?.Invoke();
    }

    public void CompleteQuest(QuestData questData)
    {
        DictOfActiveQuests.Remove(questData.Id);

        int numberOfCompletdQuest = 0;

        foreach (var item in ActiveQuests.ToList())
        {
            if (item.claimed)
            {
                numberOfCompletdQuest++;
            }
        }

        if (numberOfCompletdQuest == 3)
        {
            LoadingNewQuests?.Invoke();
            AllQuestsCompleted = true;
        }
    }
    //======================================================================================================================================================
    private void Update()
    {
        if (ResetTimer > 0 && AllQuestsCompleted)
        {
            ResetTimer -= Time.deltaTime;
        }
        else if (ResetTimer <= 0 && AllQuestsCompleted)
        {
            AllQuestsCompleted = false;
            ResetTimer = 2f;
            GenerateNewObjectives();
        }
    }
    //======================================================================================================================================================
    public void GenerateNewObjectives()
    {
        foreach (var item in ActiveQuests.ToList())
        {
            ActiveQuests.Remove(item);
            InitializeQuests();
        }
    }
    //======================================================================================================================================================
    [ContextMenu("Generate New Challenges")]
    public void GeneratedQuest()
    {
        GenerateNewObjectives();
    }
    //======================================================================================================================================================
    public void SetQuestProgressByType(QuestTypeEnum type, int progress)
    {
        QuestData objectiveData = GetOnGoingObjectiveById(type);
        if (objectiveData != null)
        {
            objectiveData.UpdateProgress(progress);

            Notification notification = new Notification();
            notification.Name = objectiveData.Id;
            objectiveData.Description = objectiveData.Description.Replace("%", "" + objectiveData.requirment);
            notification.Description = objectiveData.Description.Replace(" X ", "" + progress);
            notificationService?.Add(notification);
        }
        OnQuestValueChanged?.Invoke();
    }
    //======================================================================================================================================================
    [ContextMenu("Finish Quests")]
    public void FinishQuest()
    {
        for (int i = 0; i < ActiveQuests.Count; i++)
        {
            QuestData objective = ActiveQuests[i];
            objective.UpdateProgress(objective.requirment);
        }
        OnQuestValueChanged?.Invoke();
    }
    //======================================================================================================================================================
    public QuestData GetOnGoingObjectiveById(QuestTypeEnum objectiveType)
    {
        for (int i = 0; i < ActiveQuests.Count; i++)
        {
            if ((QuestTypeEnum)ActiveQuests[i].questType == objectiveType)
            {
                return ActiveQuests[i];
            }
        }
        return null;
    }

    private void OnQuestProgressHandled(QuestProgressEvent payload)
    {
        SetQuestProgressByType(payload.questTypeEnum, payload.value);
    }
}