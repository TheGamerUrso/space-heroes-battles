using System;
using UnityEngine;

[Serializable]
public class ShipData
{
    [Range(1,25)]
    public int Level;
    public float Health;
    public float Damage;
    public float FireRate;
    public float Speed;
    public bool HasShield;

    public ShipData()
    {

    }
}
