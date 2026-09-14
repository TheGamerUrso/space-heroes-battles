using System;
using System.Collections.Generic;
using System.Linq;
using TheGamerUrso.Core;
using UnityEngine;

public class QuestUI : UIView
{
    [SerializeField] private GameObject[] questLocation;
    protected IDataService dataService;
    protected IQuestService questService;
    private void OnEnable()
    {
        RefreshObjectives();
    }
    public virtual void Awake()
    {
        dataService = GameContext.Get<IDataService>();

        questService = GameContext.Get<IQuestService>();
        questService.LoadingNewQuests += LoadingNewQuests;
        questService.OnNewQuestGenerated += InitializeObjectives;
        questService.OnQuestValueChanged += RefreshObjectives;
    }

    private void OnDestroy()
    {
        questService.LoadingNewQuests -= LoadingNewQuests;
        questService.OnNewQuestGenerated -= InitializeObjectives;
        questService.OnQuestValueChanged -= RefreshObjectives;
    }

    private void Start()
    {    
        InitializeObjectives();
    }

    public void LoadingNewQuests()
    {
        GameObject questGO;
        for (int i = 0; i < questLocation.Length; i++)
        {
            questGO = questLocation[i];
            questGO.GetComponent<QuestUIElement>().LoadingIndicator();
        }
    }

    public void RefreshObjectives()
    {
        GameObject questGO;
        for (int i = 0; i < questService.ListOfActiveQuest.Count; i++)
        {
            questGO = questLocation[i];
            questGO.GetComponent<QuestUIElement>().RefreshQuests();
        }
    }

    public void InitializeObjectives()
    {
        var questIndex = 0;
        foreach (QuestData item in dataService.GetPlayerData().ListOfPlayerActiveQuest.ToList())
        {
            var questLocation = this.questLocation[questIndex];
            var questUIElement = questLocation.GetComponent<QuestUIElement>();
            questUIElement.InitializeObjective(item);
            questIndex++;
        }
        RefreshObjectives();
    }
}