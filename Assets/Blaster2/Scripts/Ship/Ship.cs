using System;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{

    [SerializeField] protected AudioSource audioSource;
    public HealthComponent healthComponent;
    public WeaponController weaponController;
    public BaseMovementController movementController;

    [Header("STATS")]
    public abstract ShipData ShipData { get;}
    public abstract Ship_SO Ship_SO { get; }
    public GameObject ShieldEffect;
    public abstract void SetStats(int level);
    public virtual void Idle() { }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Combat() { }
    public virtual void Death() { }
    public virtual void Hit() { }

    public virtual void ActiveShield()
    {
        ShipData.HasShield = true;
        if (ShieldEffect != null) ShieldEffect.SetActive(GetShipData<ShipData>().HasShield);
    }
    public virtual void DeactivateShield()
    {
        ShipData.HasShield = false;
        if (ShieldEffect != null) ShieldEffect.SetActive(GetShipData<ShipData>().HasShield);
    }

    public T GetShipData<T>() where T : ShipData
    {
        return (T)ShipData;
    }
}