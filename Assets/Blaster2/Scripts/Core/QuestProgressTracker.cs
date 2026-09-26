using System;
using TheGamerUrso.Core;
using UnityEditor.Overlays;
using UnityEngine;

public class QuestProgressTracker : MonoBehaviour
{
    private IQuestService questService;
    private IEventService eventService;
    
    void Start()
    {
        eventService = GameContext.Get<IEventService>();
        questService = GameContext.Get<IQuestService>();

        eventService.Subscribe<QuestProgressEvent>(OnQuestProgressHandled);

    }
    private void OnDestroy()
    {
        eventService.Unsubscribe<QuestProgressEvent>(OnQuestProgressHandled);
    }

    private void OnQuestProgressHandled(QuestProgressEvent payload)
    {
        questService.SetQuestProgressByType(payload.questTypeEnum, payload.value);
    }  
}
