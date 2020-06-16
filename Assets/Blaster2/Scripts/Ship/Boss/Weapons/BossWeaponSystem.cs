using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossWeaponSystem : WeaponScript
{
    //===========================================================================
    public void Phases()
    {
        float m_CurHealth = GetComponent<Ship>().CurrentHealth;
        float m_MaxHealth = GetComponent<Ship>().MaxHealth;

        if (m_CurHealth <= (0.6 * m_MaxHealth))
        {
            SetFireRate(2);
           // if (!SecondAttack)
           // {
                //SecondAttack = true;
                //m_MultishotWeapon.enabled = true;
           // }

        }
        else if (m_CurHealth <= (0.4 * m_MaxHealth))
        {
            SetFireRate(3);
            //ThirdAttack();
        }
        else
        {
        }

    }

    //===========================================================================
    public override void Shoot()
    {


    }
}
