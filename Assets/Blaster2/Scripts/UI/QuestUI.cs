using System.Collections.Generic;
using System.Linq;
using TheGamerUrso.Core;
using UnityEngine;

public class QuestUI : UIView
{
    [SerializeField] private QuestUIElement[] questElements;
    [SerializeField] protected IQuestService questService;

    private void OnDestroy()
    {
        questService.LoadingNewQuests -= LoadingNewQuests;
        questService.OnQuestsValueChanged -= RefreshQuests;

        for (int i = 0; i < questElements.Length; i++)
        {
            questElements[i].OnButtonPressed -= QuestUI_OnButtonPressed;
        }
    }

    private void Start()
    {
        questService = GameContext.Get<IQuestService>();
        questService.LoadingNewQuests += LoadingNewQuests;
        questService.OnQuestsValueChanged += RefreshQuests;

        for (int i = 0; i < questElements.Length; i++)
        {
            questElements[i].OnButtonPressed += QuestUI_OnButtonPressed;
        }

        RefreshQuests(questService.GetActiveQuest());
    }

    private void QuestUI_OnButtonPressed(int Index)
    {
        questService.CompleteQuest(Index);
    }

    public void LoadingNewQuests()
    {
        for (int i = 0; i < questElements.Length; i++)
        {
            questElements[i].LoadingIndicator();
        }
    }

    public void RefreshQuests(IReadOnlyDictionary<int, QuestData> ListOfActiveQuest)
    {
        foreach (KeyValuePair<int, QuestData> item in ListOfActiveQuest.ToList())
        {
            var questElement = questElements[item.Key];
            questElement.SetElement(item.Key, item.Value);
        }
    }
}