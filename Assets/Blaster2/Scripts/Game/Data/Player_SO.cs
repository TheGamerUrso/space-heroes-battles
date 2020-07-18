using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Player", menuName = "New Player")]
public class Player_SO : ScriptableObject
{
    public int MaxLevel;

    public float baseHealth;

    public float baseSpeed;
    public float baseFireRate;
    public float baseDamage;
    public float baseSpecialCountdown;
    public float baseSuperDamage;

    public bool CanUsePowerUpItem;

    public PoolGameObjectType ExplostionEffect;

    public AudioClip powerSFX;
    public AudioClip alarmSFX;
    public AudioClip hitSFX;

}