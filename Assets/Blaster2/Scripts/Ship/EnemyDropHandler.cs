using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public class EnemyDropHandler : MonoBehaviour
{
    public HealthComponent healthComponent;
    protected IEventService eventService;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eventService = GameContext.Get<IEventService>();
        healthComponent.OnDeath += HealthComponent_OnDeath;
    }

     private void OnDestroy()
    {
        healthComponent.OnDeath -= HealthComponent_OnDeath;
    }

    private void HealthComponent_OnDeath()
    {
        eventService.Publish(new DropRandomItemEvent() { SpawnPosition = transform });
    }

}
