using System;
using UnityEngine;
using TheGamerUrso;

public class Enemy : BaseEnemy
{
    public override void ShipSetup()
    {
        base.ShipSetup();
        id = gameObject.name;
    }
}