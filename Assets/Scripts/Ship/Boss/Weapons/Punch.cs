using System;
using System.Collections;
using UnityEngine;

public class Punch : Ship, IDestroyable
{
    public event Action<object> OnEnemyHit = delegate { };
    public Transform[] Waypoints;


    public HealthBarSettings HealthBarSettings;
    private EnemyHealthWidget healthBar;

    public int currentWaypoint;
    public float cooldown;
    public int delay;

    public bool isDestroyed = false;



    protected float takeDamageDelay;
    public GameObject FireEffect;

    public BaseBossEnemy baseBossEnemy;

    #region Getters and Setters
    public bool IsDestroyed
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
    #endregion

    
    private void Update()
    {
      
        var info = animator.GetCurrentAnimatorStateInfo(0);

        if (info.shortNameHash == Animator.StringToHash("Enter")){
            return;
        }

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
        yield return new WaitForSeconds(UnityEngine.Random.Range(2,3));
        currentWaypoint = 0;
        cooldown = UnityEngine.Random.Range(2, 4);
    }

    public void TakeDamage(float dmg)
    {
        if (GuiManager.IsTrasnmiting())
        {
            return;
        }
        CurrentHealth -= dmg;
        Debug.Log(GetHealthPresentage());
        if (GetHealthPresentage() <= 50)
        {
            if (FireEffect && !FireEffect.activeSelf)
                FireEffect.SetActive(true);
        }

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            if (isDestroyed == false)
            {
                Death();
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
        CurrentHealth += ammount;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public override void ShipStartSetUp()
    {
        if (cooldown >= 0)
        {
            cooldown = UnityEngine.Random.Range(2, 4);
        }
        else
        {
            cooldown = 0;
        }
        GetShipStatsSystem().ReplaceBaseStats(baseBossEnemy.GetShipStatsSystem());

        GetLevelSystem().SetLevel(baseBossEnemy.GetLevelSystem().GetLevel());

        GetShipStatsSystem().SetStats(levelSystem);


        if (FireEffect)
            FireEffect.SetActive(false);

        if (HealthBarSettings != null)
        {   
            GameObject initializedHealthWidget = Instantiate(HealthBarSettings.HealthBarPrefab, transform, false);
            healthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            OnEnemyHit += initializedHealthWidget.GetComponent<BaseHealthWidget>().OnDamageTaken;
        }
    }

    public override void InitReferences()
    {
        animator = baseBossEnemy.GetAnimator();
    }

    public override void Death()
    {
        GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
        explostion.transform.position = transform.position;
        isDestroyed = true;

    }
}