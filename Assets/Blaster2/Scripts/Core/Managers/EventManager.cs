using System;
using UnityEngine;

namespace TheGamerUrso.Core
{
    /// <summary>
    /// Core scene service wrapper exposing global EventBus functionality via GameContext.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public class EventManager : ServiceComponent<IEventService>, IEventService
    {
        //====================================================================================================
        // Serialized Fields
        //====================================================================================================
        [Header("Configuration")]
        [SerializeField] private EventBus eventBus;
        //====================================================================================================
        // Public API Methods
        //====================================================================================================
        public void Subscribe<T>(Action<T> listener)
        {
            if (eventBus == null) return;
            eventBus.Subscribe(listener);
        }
        //====================================================================================================
        public void Unsubscribe<T>(Action<T> listener)
        {
            if (eventBus == null) return;
            eventBus.Unsubscribe(listener);
        }
        //====================================================================================================
        public void Publish<T>(T evt)
        {
            if (eventBus == null) return;
            eventBus.Publish(evt);
        }
        //====================================================================================================
        public void ClearAll()
        {
            if (eventBus == null) return;
            eventBus.ClearAll();
        }
    }
}

