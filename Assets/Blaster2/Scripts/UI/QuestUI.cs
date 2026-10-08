using System.Linq;
using TheGamerUrso.Core;
using UnityEngine;

public class QuestUI : UIView
{
    [SerializeField] private GameObject[] questLocation;
    [SerializeField] protected IQuestService questService;

    private void OnDestroy()
    {
        questService.LoadingNewQuests -= LoadingNewQuests;
        questService.OnNewQuestGenerated -= InitializeObjectives;
        questService.OnQuestValueChanged -= RefreshObjectives;
    }

    private void Start()
    {
        questService = GameContext.Get<IQuestService>();
        questService.LoadingNewQuests += LoadingNewQuests;
        questService.OnNewQuestGenerated += InitializeObjectives;
        questService.OnQuestValueChanged += RefreshObjectives;

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
        foreach (QuestData item in questService.ListOfActiveQuest.ToList())
        {
            var questLocation = this.questLocation[questIndex];
            var questUIElement = questLocation.GetComponent<QuestUIElement>();
            questUIElement.InitializeObjective(item);
            questIndex++;
        }
        RefreshObjectives();
    }
}