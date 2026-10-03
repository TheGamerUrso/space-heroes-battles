using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject LevelUpPrefab;
    private IEventService eventService;

    private void Start()
    {
        eventService = GameContext.Get<IEventService>();
        eventService.Subscribe<PlayerStatsUpdatedEvent>(PlayerStatsUpdateEventHandled);
    }

    protected void OnDestroy()
    {
        eventService.Unsubscribe<PlayerStatsUpdatedEvent>(PlayerStatsUpdateEventHandled);
    }

    public void OnLevelValueChanged(int Level)
    {
  
    }  
    //=================================================================================
    public void PlayerStatsUpdateEventHandled(PlayerStatsUpdatedEvent payload)
    {
        switch (payload.type)
        {
            case PlayerStatsUpdatedEvent.StatType.Level:
                LevelUpPrefab.SetActive(true);
                break;
        }

    }
}
