using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossWeaponSystem : WeaponScript
{   
    public void Phases()
    {
        float m_CurHealth = GetComponent<IDamagable>().CurrentHealth;
        float m_MaxHealth = GetComponent<IDamagable>().MaxHealth;

        if (m_CurHealth <= (0.6 * m_MaxHealth))
        {
            FireRate = 2;

        }
        else if (m_CurHealth <= (0.4 * m_MaxHealth))
        {
            FireRate = 3;
        }
        else
        {
        }
    }
}
