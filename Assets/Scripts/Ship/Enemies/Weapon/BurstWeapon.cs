using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class BurstWeapon : Blaster
{
    public int repeat;
    public bool m_Shooting;

    public override void Shoot()
    {
        if (!m_Shooting)
        {
            m_Shooting = true;
        }
    }

}