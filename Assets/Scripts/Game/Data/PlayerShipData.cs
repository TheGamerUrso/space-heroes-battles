using System;
using UnityEngine;


[System.Serializable]
public class PlayerShipData
{
    public int MaxLevel = 20;

    public int level;
    public float xp;
    public float xpToLevel;
    public int[] Upgrades;

    public PlayerShipData()
    {
        MaxLevel = 20;
        level = 1;
        xp = 0;
        xpToLevel = 100;
        Upgrades = new int[Enum.GetValues(typeof(UpgradeType)).Length - 1];
    }
}
