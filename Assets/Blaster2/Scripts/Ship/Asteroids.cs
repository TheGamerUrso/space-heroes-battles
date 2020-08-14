using System;
using UnityEngine;

public class Asteroids : MonoBehaviour, IDamagable
{
    public Action<float, float> HealthChanged;

    public bool Destroyed;
    public float currentHealth;
    public float maxHealth;
    public float yVel;
    public float speed;
    public float rotSpeed;
    public Transform AsteroidTransform;
    public float takeDamageDelay;

    public event Action<float, float> OnHealthChanged;

    public bool IsDestroyed
    {
        get
        {
            return Destroyed;
        }
        set { Destroyed = value; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }
    public Action<float, float> OnHealthChange
    {
        get
        {
            return HealthChanged;
        }
        set
        {
            HealthChanged = value;
        }
    }
    private void Start()
    {
        rotSpeed = UnityEngine.Random.Range(50, 100);
        currentHealth = maxHealth;
    }

    public void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        transform.Translate(-transform.forward * speed * Time.deltaTime);

        AsteroidTransform.Rotate(Vector3.right, rotSpeed * Time.deltaTime);

        if (transform.position.z < Constants.m_ZMin)
        {
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float dmg)
    {
        if (Destroyed == true)
        {
            return;
        }
    }

    public void SetRandomPosition()
    {
        System.Random rand = new System.Random();
        Vector3 newPos = new Vector3(rand.Next((int)Constants.m_XMin, (int)Constants.m_XMax), -50, rand.Next((int)Constants.m_ZMax, (int)(Constants.m_ZMax + 50)));

        yVel = rand.Next(-15, 15);
        transform.position = newPos;

        float dist = (transform.position - new Vector3(0, -50, transform.position.z)).magnitude;
        dist = Mathf.Clamp(dist, -5, 5);

        if (transform.position.x < 0)
        {
            dist *= -1;
        }

        transform.rotation = Quaternion.Euler(0, dist, 0);
    }

    public void Heal(float ammount)
    {
   
    }

    public void Death()
    {
        
    }
}