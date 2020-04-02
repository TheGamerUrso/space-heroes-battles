using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ShipStat", menuName = "New ShipStat")]
public class ShipStats : ScriptableObject
{
    /**
     * Base Attributes
     */
    public float baseDamage;
    public float baseHealth;
    public float baseFireRate;
    public float baseSpecialCountdown;

    public float baseSpeed;
}