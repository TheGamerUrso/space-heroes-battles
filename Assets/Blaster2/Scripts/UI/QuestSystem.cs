using NUnit.Framework.Interfaces;
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
    public event Action<Dictionary<int, QuestData>> OnNewQuestGenerated;
    public event Action OnQuestValueChanged;
    [SerializeField] private Dictionary<int, QuestData> ActiveQuestDict = new Dictionary<int, QuestData>();
    private float ResetTimer = 2f;
    private bool AllQuestsCompleted = false;

    private INotificationService notificationService;
    private IQuestService questService;
    private IEventService eventService;
    private IDataService dataService;

    private PlayerData playerData;
    private PlayerShipData playerShipData;

    private void Start()
    {
        eventService = GameContext.Get<IEventService>();
        notificationService = GameContext.Get<INotificationService>();

        eventService.Subscribe<QuestProgressEvent>(OnQuestProgressHandled);
        OnQuestValueChanged?.Invoke();

        dataService = GameContext.Get<IDataService>();

        playerData = dataService.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        playerData.OnCurrentShipSelectedValueChanged += PlayerData_OnCurrentShipSelectedValueChanged;

        GeneratedQuest();
    }

    private void PlayerData_OnCurrentShipSelectedValueChanged(int obj)
    {
        playerShipData = playerData.GetCurrentPlayerShipData();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        eventService.Unsubscribe<QuestProgressEvent>(OnQuestProgressHandled);

        playerData.OnCurrentShipSelectedValueChanged -= PlayerData_OnCurrentShipSelectedValueChanged;
    }

    public void CreateNewQuests(int questIndex)
    {
        var ListOfAvailableQuestType = Enum.GetValues(typeof(QuestTypeEnum)).Cast<QuestTypeEnum>().ToList();
 
        int randoNumber = UnityEngine.Random.Range(0, ListOfAvailableQuestType.Count);
        var questType = ListOfAvailableQuestType[randoNumber];
        ListOfAvailableQuestType.Remove(questType);

        if(ActiveQuestDict.TryGetValue(questIndex,out QuestData questDataFound))
        {
            ActiveQuestDict[questIndex] = CreateQuest(questIndex, questType);
        }
        else
        {
            ActiveQuestDict.Add(questIndex, CreateQuest(questIndex, questType));
        }
        OnNewQuestGenerated?.Invoke(ActiveQuestDict);
    }
    //======================================================================================================================================================
    public QuestData CreateQuest(int pos, 
        QuestTypeEnum questType)
    {
        var description = "QUEST_DESCRIPTION_ID";
        var requirment = 0;
        var xpReward = 0.0f;
        var CurrencyReward = 0;
        switch (questType)
        {
            case QuestTypeEnum.KILL:
                description = "Kill <color=orange> X / % </color>enemies";
                requirment = UnityEngine.Random.Range(200, 500);
                xpReward = 50f;
                CurrencyReward = 100;
                break;
            case QuestTypeEnum.USE:
                description = "Use super <color=orange> X / % </color>  times";
                requirment = UnityEngine.Random.Range(3, 10);
                xpReward = 20f;
                CurrencyReward = 40;
                break;
            case QuestTypeEnum.UNHARMED:
                description = "Achieve <color=orange> % </color> Score Without Getting Hit";
                requirment = UnityEngine.Random.Range(200, 500);
                xpReward = 75f;
                CurrencyReward = 150;
                break;
            case QuestTypeEnum.SURVIVE:
                description = "Survive <color=orange> % </color> Waves";
                requirment = UnityEngine.Random.Range(10, 50);
                xpReward = 25f;
                CurrencyReward = 50;
                break;
            case QuestTypeEnum.SPEND:
                description = "Spend <color=orange> X / % </color> coins";
                requirment = UnityEngine.Random.Range(500, 1000);
                xpReward = 15f;
                CurrencyReward = 0;
                break;
            case QuestTypeEnum.BOUNTY:  
                description = "Defeat a strong Enemy";
                requirment = 1;
                xpReward = 100f;
                CurrencyReward = 200;
                break;
            case QuestTypeEnum.SCORE:
                description = "Achieve <color=orange> % </color> Total Score";
                requirment = UnityEngine.Random.Range(10000, 50000);
                xpReward = 80f;
                CurrencyReward = 100;
                break;
        }

        return new QuestData(pos,
                    requirment,
                    questType,
                    description, 
                    new QuestReward(xpReward, CurrencyReward));
    }
    //======================================================================================================================================================
    public void CompleteQuest(int questIndex)
    {
        var questData = ActiveQuestDict[questIndex];
        if (!questData.claimed && questData.completed)
        {
            playerData.playerEconomyData.Coins += questData.questReward.Currency;
            playerData.GetCurrentPlayerShipData().EarnXP(questData.questReward.XP);

            switch ((QuestTypeEnum)questData.questType)
            {
                case QuestTypeEnum.KILL:
                    playerData.playerStatsData.UpdateKills(0);
                    break;
                case QuestTypeEnum.USE:
                    playerData.playerStatsData.UpdateSuperUsed(0);
                    break;
                case QuestTypeEnum.UNHARMED:
                    playerData.playerStatsData.UpdateIsUnharmed(false);
                    break;
                case QuestTypeEnum.SURVIVE:
                    playerData.playerStatsData.UpdateWaveSurvived(0);
                    break;
                case QuestTypeEnum.SPEND:
                    playerData.playerEconomyData.UpdateSpendCurrency(0);
                    break;
                case QuestTypeEnum.BOUNTY:
                    playerData.playerStatsData.UpdateBounty(0);
                    break;
                case QuestTypeEnum.SCORE:
                    playerData.playerStatsData.UpdateScore(0);
                    break;
            }

            OnQuestValueChanged?.Invoke();

            int numberOfCompletdQuest = 0;

            foreach (var item in ActiveQuestDict.ToList())
            {
                if (item.Value.claimed)
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
            GeneratedQuest();
        }
    }
    //======================================================================================================================================================
    public void SetQuestProgressByType(QuestTypeEnum type, int progress)
    {
        QuestData questData = GetOnGoingObjectiveById((int)type);
        if (questData != null)
        {
            questData.UpdateProgress(progress);

            Notification notification = new Notification();
            notification.Name = questData.Id;
            questData.Description = questData.Description.Replace("%", "" + questData.requirment);
            notification.Description = questData.Description.Replace(" X ", "" + progress);
            notificationService?.Add(notification);
        }
        OnQuestValueChanged?.Invoke();
    }
    //======================================================================================================================================================
    [ContextMenu("Generate New Challenges")]
    public void GeneratedQuest()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateNewQuests(i);
        }
    }
    //======================================================================================================================================================
    [ContextMenu("Finish Quests")]
    public void FinishQuest()
    {
        foreach (var item in ActiveQuestDict.Values)
        {
            item.UpdateProgress(item.requirment);
        }

    }
    //======================================================================================================================================================
    public QuestData GetOnGoingObjectiveById(int questIndex) =>  ActiveQuestDict[questIndex];
    private void OnQuestProgressHandled(QuestProgressEvent payload) => SetQuestProgressByType(payload.questTypeEnum, payload.value);
    public IReadOnlyDictionary<int, QuestData> GetActiveQuest() => ActiveQuestDict;
}