using System;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestService 
{
    public event Action LoadingNewQuests;
    public event Action<Dictionary<int, QuestData>> OnQuestsValueChanged;
    public IReadOnlyDictionary<int,QuestData> GetActiveQuest();

    public void SetQuestProgressByType(QuestTypeEnum type, int progress);
    void CompleteQuest(int QuestIndex);
}
