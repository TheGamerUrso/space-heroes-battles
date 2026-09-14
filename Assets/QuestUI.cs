using System.Linq;
using TheGamerUrso.Core;
using UnityEngine;

public class QuestUI : UIView
{
    [SerializeField] private GameObject[] questLocation;
    [SerializeField] protected QuestSystem questManager;

    private void OnDestroy()
    {
        questManager.LoadingNewQuests -= LoadingNewQuests;
        questManager.OnNewQuestGenerated -= InitializeObjectives;
        questManager.OnQuestValueChanged -= RefreshObjectives;
    }

    private void Start()
    {
        questManager.LoadingNewQuests += LoadingNewQuests;
        questManager.OnNewQuestGenerated += InitializeObjectives;
        questManager.OnQuestValueChanged += RefreshObjectives;

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
        for (int i = 0; i < questManager.ListOfActiveQuest.Count; i++)
        {
            questGO = questLocation[i];
            questGO.GetComponent<QuestUIElement>().RefreshQuests();
        }
    }

    public void InitializeObjectives()
    {
        var questIndex = 0;
        foreach (QuestData item in questManager.ActiveQuests.ToList())
        {
            var questLocation = this.questLocation[questIndex];
            var questUIElement = questLocation.GetComponent<QuestUIElement>();
            questUIElement.InitializeObjective(item);
            questIndex++;
        }
        RefreshObjectives();
    }
}