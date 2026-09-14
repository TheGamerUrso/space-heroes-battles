using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheGamerUrso.Core
{
    /// <summary>
    /// Type-safe runtime event channel backing the EventManager infrastructure.
    /// </summary>
    [CreateAssetMenu(fileName = "EventBus", menuName = "TheGamerUrso/Core/Event Bus")]
    public class EventBus : ScriptableObject
    {
        private readonly Dictionary<Type, Delegate> eventTable = new Dictionary<Type, Delegate>();
        //====================================================================================================
        private void OnDisable()
        {
            ClearAll();
        }
        //====================================================================================================
        public void Subscribe<T>(Action<T> listener)
        {
            if (listener == null) return;

            Type eventType = typeof(T);

            if (eventTable.TryGetValue(eventType, out Delegate existingDelegate))
            {
                eventTable[eventType] = Delegate.Combine(existingDelegate, listener);
            }
            else
            {
                eventTable[eventType] = listener;
            }
        }
        //====================================================================================================
        public void Unsubscribe<T>(Action<T> listener)
        {
            if (listener == null) return;

            Type eventType = typeof(T);

            if (eventTable.TryGetValue(eventType, out Delegate existingDelegate))
            {
                Delegate currentDelegate = Delegate.Remove(existingDelegate, listener);

                if (currentDelegate == null)
                {
                    eventTable.Remove(eventType);
                }
                else
                {
                    eventTable[eventType] = currentDelegate;
                }
            }
        }
        //====================================================================================================
        public void Publish<T>(T evt)
        {
            Type eventType = typeof(T);

            if (eventTable.TryGetValue(eventType, out Delegate existingDelegate))
            {
                (existingDelegate as Action<T>)?.Invoke(evt);
            }
        }
        //====================================================================================================
        public void ClearAll()
        {
            eventTable.Clear();
        }
    }
}
