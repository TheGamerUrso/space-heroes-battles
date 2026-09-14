using System;
namespace TheGamerUrso.Core
{
    /// <summary>
    /// Contract for decoupled, type-safe global event publishing and listening.
    /// </summary>
    public interface IEventService
    {
        void Subscribe<T>(Action<T> listener);
        void Unsubscribe<T>(Action<T> listener);
        void Publish<T>(T evt);
        void ClearAll();
    }
}
