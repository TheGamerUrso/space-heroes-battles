using UnityEngine;

public class Turret : MonoBehaviour
{
    public int Health = 1;
    public float TTL = 10;

    public PlayerShip player;

    public PlayerWeapon playerWeapon;

    public void SetShipStats(PlayerShip player)
    {
        this.player = player;
        SetDamage();
    }

    public void SetDamage()
    {
        playerWeapon.SetDamage(player.Damage / 2);
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