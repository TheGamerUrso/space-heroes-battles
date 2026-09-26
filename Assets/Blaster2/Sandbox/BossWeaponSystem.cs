using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossWeaponSystem : WeaponScript
{   
    public void Phases()
    {
        float m_CurHealth = GetComponent<HealthComponent>().GetHealthPresentage();

        if (m_CurHealth <= 0.6)
        {
            FireRate = 2;

        }
        else if (m_CurHealth <= 0.4)
        {
            FireRate = 3;
        }
        else
        {
        }
    }
}
