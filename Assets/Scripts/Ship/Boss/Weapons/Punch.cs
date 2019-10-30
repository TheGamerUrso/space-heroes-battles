using System;
using System.Collections;
using UnityEngine;

public class Punch : MonoBehaviour, IDestroyable
{
    public event Action<object> OnEnemyHit = delegate { };
    public Transform[] Waypoints;

    public HealthBarSettings HealthBarSettings;

    private GameObject healthBar;

    public int currentWaypoint;
    public float cooldown;
    public int delay;

    public bool isDestroyed = false;

    public float currentHealth;
    public float maxhealth;

    protected float takeDamageDelay;
    public GameObject FireEffect;

    public bool IsAlive
    {
        get
        {
            return isDestroyed;
        }
        set
        {
            isDestroyed = value;
        }
    }

    public float MaxHealth
    {
        get
        {
            return maxhealth;
        }
        set { maxhealth = value; }
    }

    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
        }
    }

    private void Start()
    {

        if (cooldown >= 0)
        {
            cooldown = UnityEngine.Random.Range(2, 4);
        }
        else
        {
            cooldown = 0;
        }

        if (FireEffect)
            FireEffect.SetActive(false);

        if (HealthBarSettings != null)
        {
            healthBar = Instantiate(HealthBarSettings.HealthBarPrefab, transform, false);
            OnEnemyHit += healthBar.GetComponent<BaseHealthWidget>().OnDamageTaken;
        }
    }

    private void Update()
    {
        if (isDestroyed == false)
        {
            float dist = (transform.position - Waypoints[0].position).magnitude;

            if (dist <= 1)
            {
                cooldown -= Time.deltaTime;
                if (cooldown <= 0)
                {
                    if (GuiManager.IsTrasnmiting())
                    {
                        return;
                    }
                    StartCoroutine(PunchCoroutine());
                }
            }

            if (takeDamageDelay >= 0)
            {
                takeDamageDelay -= Time.deltaTime;
            }
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[currentWaypoint].position, 1);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[0].position, 1);
        }
    }

    private IEnumerator PunchCoroutine()
    {
        currentWaypoint = 1;
        yield return new WaitForSeconds(1.0f);
        currentWaypoint = 0;
        cooldown = UnityEngine.Random.Range(2, 4);
    }

    public void TakeDamage(float dmg)
    {
        if (GuiManager.IsTrasnmiting())
        {
            return;
        }
        currentHealth -= dmg;
        if (CurrentHealth <= 0)
        {
            currentHealth = 0;
            if (isDestroyed == false)
            {
                GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
                explostion.transform.position = transform.position;
                isDestroyed = true;
                if (FireEffect)
                    FireEffect.SetActive(true);
            }
        }

        OnEnemyHit?.Invoke(this);

    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        string gameobjectTag = other.gameObject.tag;

        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            if (takeDamageDelay <= 0)
            {
                takeDamageDelay = .2f;

                TakeDamage(12.5f);
            }
        }

        PlayerProjectile playerProjectile = other.GetComponent<PlayerProjectile>();

        if (playerProjectile)
        {
            if (takeDamageDelay <= 0)
            {
                takeDamageDelay = .2f;

                if (playerProjectile)
                {
                    TakeDamage(playerProjectile.getDamage());
                }
            }
        }
    }

    public void Heal(float ammount)
    {
        currentHealth += ammount;

        if (currentHealth > maxhealth)
        {
            currentHealth = maxhealth;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxhealth);
    }
}