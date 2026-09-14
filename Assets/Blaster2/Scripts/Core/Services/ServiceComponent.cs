using UnityEngine;

namespace TheGamerUrso.Core
{
    /// <summary>
    /// Abstract MonoBehaviour backing component that auto-registers implementing interfaces into GameContext.
    /// </summary>
    public abstract class ServiceComponent<T> : MonoBehaviour where T : class
    {
        //====================================================================================================
        protected virtual void Awake()
        {
            T service = this as T;

            if (service == null)
            {
                Debug.LogError($"[{nameof(ServiceComponent<T>)}] Component '{gameObject.name}' does not implement service interface '{typeof(T).Name}'!");
                return;
            }

            GameContext.Register(service);
        }
        //====================================================================================================
        protected virtual void OnDestroy()
        {
            if (this as T != null)
            {
                GameContext.Unregister(this as T);
            }
        }
    }
}
