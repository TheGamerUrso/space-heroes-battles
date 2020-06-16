using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    private static bool _shuttingDown;

    public static T Instance
    {
        get
        {
            if (_shuttingDown)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed.");
                return null;
            }

            // Check if an instance has been set already and if not, try to find an instance of this singleton
            // in the scene; this may happen, when a script tries to access the instance during Awake, which means
            // the instance may not have been initialized, yet.
            _instance = _instance ? _instance : FindObjectOfType<T>();
            if (!_instance)
            {
                Debug.LogError($"[Singleton] No instance of singleton '{typeof(T)}' found!");
            }

            return _instance;
        }

        private set => _instance = value;
    }

    protected void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning($"[Singleton] A second instance of a singleton '{typeof(T)}' is not allowed.", this);

            Destroy(gameObject);
            return;
        }

        Instance = (T)this;
        OnAwake();
    }

    private void OnDestroy()
    {
        if (_shuttingDown)
        {
            return;
        }

        OnCleanup();

        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Called during Awake. Overwrite for custom behavior during the Awake event.
    /// </summary>
    protected virtual void OnAwake()
    {
    }

    /// <summary>
    /// Called during OnDestroy. Overwrite for custom behavior during the OnDestroy event.
    /// Note: This function is NOT called during shut down (i.e. when quitting the application).
    /// </summary>
    protected virtual void OnCleanup()
    {
    }

    private void OnApplicationQuit()
    {
        _shuttingDown = true;
        Instance = null;
    }
}