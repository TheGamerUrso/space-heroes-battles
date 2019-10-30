using UnityEngine;

public class Turret : MonoBehaviour
{
    public int Health = 1;
    public float TTL = 10;

    public ShipStats shipStats;
    public PlayerWeapon playerWeapon;

    public void SetShipStats(ShipStats shipStats)
    {
        this.shipStats = shipStats;
        SetDamage();
    }

    public void SetDamage()
    {
        playerWeapon.SetDamage(shipStats.Damage / 2);
    }

    private void Update()
    {
        TTL -= Time.deltaTime;
        if (TTL < 0 || Health < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.ENEMYTAG))
        {
            Health--;
        }
        if (other.tag.Equals(Constants.ENEMYPROJECTILETAG))
        {
            Health--;
        }
    }
}