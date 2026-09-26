using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthComponent : HealthComponent
{

    public override void TakeDamage(float dmg)
    {
        if(((BossEnemy)ship).IsProtected())
        if (IsAlive == false) return;


        audioSource.PlayOneShot(hitSFX);

        if (invisibilityTimer <= 0)
        {
            invisibilityTimer = .1f;

            if (HasShield)
            {
                HasShield = false;
            }
            else if (HasShield == false)
            {
               currentHealth -= dmg;

                OnHealthChanged?.Invoke(currentHealth, maxHealth);

                if (currentHealth < 1)
                {
                    if (IsAlive)
                    {
                        IsAlive = false;
                        ship.Death();
                    }
                }
            }
            ShieldEffect.SetActive(HasShield);
        }
       ship.Hit();
    }

  

}
