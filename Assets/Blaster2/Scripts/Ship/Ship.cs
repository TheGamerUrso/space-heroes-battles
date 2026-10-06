using System;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{

    [SerializeField] protected AudioSource audioSource;
    public HealthComponent healthComponent;
    public WeaponController weaponController;
    public BaseMovementController movementController;

    [Header("STATS")]
    public ShipData shipData;
    public Ship_SO ship_SO;
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
        shipData.HasShield = true;
        if (ShieldEffect != null) ShieldEffect.SetActive(shipData.HasShield);
    }
    public virtual void DeactivateShield()
    {
        shipData.HasShield = false;
        if (ShieldEffect != null) ShieldEffect.SetActive(shipData.HasShield);
    }
}