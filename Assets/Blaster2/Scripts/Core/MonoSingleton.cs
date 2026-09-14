using UnityEngine;

/// <summary>
/// Mono singleton Class. Extend this class to make singleton component.
/// Example:
/// <code>
/// public class Foo : MonoSingleton<Foo>
/// </code>. To get the instance of Foo class, use <code>Foo.instance</code>
/// Override <code>Init()</code> method instead of using <code>Awake()</code>
/// from this class.
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }
    }
    //====================================================================================================
    public static bool IsInitialized
    {
        get { return instance != null; }
    }
    //====================================================================================================
    protected void OnDestroy() => CleanUp();
    protected void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            Init();
        }
        else if (instance != null)
        {
            Debug.LogError("[Singleton]  Trying to  instansiate  a  second instance  of a  singleton class." + gameObject.name);
            Destroy(gameObject);
        }
    }
    //====================================================================================================    
    protected void Start() => Setup();
    //====================================================================================================
    protected virtual void Setup() { }
    //====================================================================================================
    protected virtual void Init() { }
    //====================================================================================================
    protected virtual void CleanUp() { }
}