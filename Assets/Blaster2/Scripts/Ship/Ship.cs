using UnityEngine;

public abstract class Ship : MonoBehaviour
{
    protected Animator animator;

    protected bool HasShield;

    [SerializeField] protected GameObject ShieldEffect;
    public virtual void OnDestroy() { }
    public virtual void OnDisable() { }
    public virtual void OnEnable() { }
    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void Update() { }
    public virtual void SetStats(int level) { }

    public virtual void InstallShieldModule()
    {
        if (HasShield)
        {
            return;
        }

        HasShield = true;
    }

    public bool HasShieldModule()
    {
        return HasShield;
    }


}