using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthComponent : HealthComponent
{
    [SerializeField] protected List<HealthComponent> DestroyableParts = new List<HealthComponent>();

    public override void TakeDamage(float dmg)
    {
        if (GameController.Instance.CurrentGameState == GameState.TRANSMISSION) return;
        if (IsAlive == false) return;

        ShieldEffect.SetActive(IsProtected());

        audioSource.PlayOneShot(hitSFX);

        if (invisibilityTimer <= 0)
        {
            invisibilityTimer = .1f;

            foreach (var part in DestroyableParts)
            {
                if (part.IsAlive)
                {
                    return;
                }
            }
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
                    Death();
                }
                //IncreasePhase();
            }
            ShieldEffect.SetActive(HasShield);
        }
        Hit();
    }

    public override void Hit()
    {
        base.Hit();
        //playerData.SetSuperMeter(playerData.PowerUpLevel + 0.15f);
    }
    public override void Death()
    {
        //.SetBool("Death", true);
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        DeathExplosions();

       SetDamagable(false);

        yield return new WaitForSeconds(4.0f);

        for (int i = 0; i < 4; i++)
        {
            eventService?.Publish(new DropRandomItemEvent() { SpawnPosition = transform });
        }

        if (IsAlive)
        {
            IsAlive = false;
            var explostion = PoolManager.Instance.GetObjectFromPool(explostionEffect);
            explostion.transform.position = transform.position;
            explostion.SetActive(true);
            eventService?.Publish(new ShakeCameraEvent() { duration = .5f });
            eventService?.Publish(new DropRandomItemEvent() { SpawnPosition = transform });
            Destroy(transform.parent.gameObject);
        }
    }

    //Boss Owned Methods
    private void DeathExplosions()
    {
        Vector3[] positions ={
                 transform.position,
                transform.position + (transform.right * 50),
                  transform.position - (transform.right * 50),
                    transform.position + (transform.forward * 50),
                      transform.position - (transform.forward * 50)
            };

        for (int i = 0; i < 5; i++)
        {
            var explostion = PoolManager.Instance.GetObjectFromPool(explostionEffect);
            explostion.transform.position = positions[i];
            explostion.SetActive(true);
        }
    }
    private bool IsProtected()
    {
        int destroyed = 0;
        if (DestroyableParts.Count > 0)
        {
            if (destroyed < DestroyableParts.Count)
            {
                return true;
            }
            else
            {
                return true;
            }

        }
        else
        {
            return false;
        }
    }

}
