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
        eventService.Subscribe<EnemyDiedEvent>(OnEnemyDiedHandled);
        eventService.Subscribe<NewWaveStartedEvent>(OnNewWaveStartedHandled);

    }
    private void OnDestroy()
    {
        eventService.Unsubscribe<QuestProgressEvent>(OnQuestProgressHandled);
    }

    private void OnQuestProgressHandled(QuestProgressEvent payload)
    {
        questService.SetQuestProgressByType(payload.questTypeEnum, payload.value);
    }   
    //=================================================================================
    public virtual void OnEnemyDiedHandled(EnemyDiedEvent enemyDied)
    {
        eventService?.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.KILL, value = enemyDied.EnemyKilled });

        if (!enemyDied.WasBoss) return;
        eventService?.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.BOUNTY, value = 1 });
    }  
    //=================================================================================
    public virtual void OnNewWaveStartedHandled(NewWaveStartedEvent waveStartedEvent)
    {
        eventService?.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.SURVIVE, value = waveStartedEvent.Wave });
    }
}
