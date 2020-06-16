using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ShipStat", menuName = "New ShipStat")]
public class ShipStats : ScriptableObject
{
    /**
     * Base Attributes
     */
    public float baseHealth;

    public float baseSpeed;
    public float baseFireRate;
    public float baseDamage;
    public float baseSpecialCountdown;
    public float baseSuperDamage;

    public bool CanUsePowerUpItem;


    public AudioClip hitSFX;

}