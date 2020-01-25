using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }
    }
    public static bool bIsInitialized;
    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
        }
        else if (instance != null)
        {
            Debug.LogWarning((T)this + "Trying to instantiate a second instance of a singleton class.");
            DestroyImmediate(gameObject);
            return;
        }

        if (!bIsInitialized)
        {
            bIsInitialized = true;
            Instance.Init();
        }
    }

    public virtual void Init()
    {

    }

    protected virtual void OnDestroy()
    {
        if (instance == null)
        {
            instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        OnQuitGame();
    }

    public virtual void OnQuitGame()
    {

    }
}
