using UnityEngine;

namespace TheGamerUrso.Core
{
    /// <summary>
    /// High-performance zero-allocation static service locator.
    /// Leverages static generic class instantiation for O(1) runtime access.
    /// </summary>
    public static class GameContext
    {
        private static class ServiceCache<T> where T : class
        {
            public static T Instance;
        }
        public static int MemoryUsage { get; private set; }

        //====================================================================================================
        public static void Register<T>(T service) where T : class
        {
            if (service == null) return;
            if (ServiceCache<T>.Instance != null)
            {
                Debug.LogWarning($"[{nameof(GameContext)}] Service '{typeof(T).Name}' is already registered!");
                return;
            }
            ServiceCache<T>.Instance = service;
            MemoryUsage++;
            Debug.Log($"[{nameof(GameContext)}] Registered service: '{typeof(T).Name}'");
        }
        //====================================================================================================
        public static void Unregister<T>(T service) where T : class
        {
            if (ServiceCache<T>.Instance == service)
            {
                ServiceCache<T>.Instance = null;
                MemoryUsage--;
                Debug.Log($"[{nameof(GameContext)}] Unregistered service: '{typeof(T).Name}'");
            }
        }
        //====================================================================================================
        public static T Get<T>() where T : class
        {
            T service = ServiceCache<T>.Instance;

            if (service == null)
            {
                Debug.LogWarning($"[{nameof(GameContext)}] Requested service '{typeof(T).Name}' is not registered!");
            }

            return service;
        }
    }
}
