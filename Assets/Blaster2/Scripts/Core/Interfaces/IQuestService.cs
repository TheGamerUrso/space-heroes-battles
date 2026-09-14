using System;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestService 
{
    public event Action LoadingNewQuests;
    public event Action OnNewQuestGenerated;
    public event Action OnQuestValueChanged;

    public List<QuestData> ListOfActiveQuest { get; }

    public void SetQuestProgressByType(QuestTypeEnum type, int progress);
    void CompleteQuest(QuestData questData);
}
