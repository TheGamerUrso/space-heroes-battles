using System;
using UnityEngine;
using TheGamerUrso;

public class Enemy : BaseEnemy
{
    public override void ShipStartSetUp()
    {
        base.ShipStartSetUp();
        id = gameObject.name;
    }
}