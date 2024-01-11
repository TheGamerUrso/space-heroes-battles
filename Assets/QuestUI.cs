using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject[] questLocation;

    private void OnEnable()
    {
        RefreshObjectives();
    }
    
    private void OnDestroy()
    {
        QuestSystem.Instance.LoadingNewQuests -= LoadingNewQuests;
        QuestSystem.Instance.OnNewQuestGenerated -= InitializeObjectives;
        QuestSystem.Instance.OnQuestValueChanged -= RefreshObjectives;
    }

    private void Start()
    {       
        QuestSystem.Instance.LoadingNewQuests += LoadingNewQuests;
        QuestSystem.Instance.OnNewQuestGenerated += InitializeObjectives;
        QuestSystem.Instance.OnQuestValueChanged += RefreshObjectives;
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
        for (int i = 0; i < QuestSystem.Instance.ListOfActiveQuest.Count; i++)
        {
            questGO = questLocation[i];
            questGO.GetComponent<QuestUIElement>().RefreshQuests();
        }
    }

    public void InitializeObjectives()
    {
        var questIndex = 0;
        foreach (QuestData item in PersistantData.GetPlayerData().ListOfPlayerActiveQuest.ToList())
        {
            var questLocation = this.questLocation[questIndex];
            var questUIElement = questLocation.GetComponent<QuestUIElement>();
            questUIElement.InitializeObjective(item);
            questIndex++;
        }
        RefreshObjectives();
    }
}