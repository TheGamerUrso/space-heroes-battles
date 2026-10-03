using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Player", menuName = "New Player",order = 0)]
public class Player_SO : Ship_SO
{
    public int MaxLevel;
    public float baseSpecialCountdown;
    public float baseSuperDamage;
    public bool CanUsePowerUpItem;
}