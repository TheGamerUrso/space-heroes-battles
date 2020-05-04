using UnityEngine;

public class Turret : MonoBehaviour, IDamagable
{
    private PlayerShipData playerShipData;
    private PlayerData playerData;

    public PlayerWeapon playerWeapon;
    public bool IsAlive;
    public bool IsDestroyed
    {
        get
        {
            return !IsAlive;
        }
        set { IsAlive = value; }
    }

    public float maxHealth;
    public float MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
        }
    }

    public float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    private void OnEnable()
    {
        if (playerData == null)
        {
            playerData = PersistantData.GetPlayerData();
            playerShipData = playerData.GetCurrentPlayerShipData();
        }

        playerWeapon.damage = playerShipData.SuperDamage;
    }

    private void Start()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
    }

    public void Deactivate()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.ENEMYTAG))
        {
            currentHealth--;
        }
        if (other.tag.Equals(Constants.ENEMYPROJECTILETAG))
        {
            currentHealth--;
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth < 0)
        {
            Destroy(gameObject);
        }
    }
}