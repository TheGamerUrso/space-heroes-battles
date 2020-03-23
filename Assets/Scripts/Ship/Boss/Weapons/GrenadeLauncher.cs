using TheGamerUrso.PoolSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrenadeLauncher : Blaster
{

    public override void Fire()
    {
        if (Time.time > newShot)
        {
            newShot = Time.time + FireRate;
            int pos = Random.Range(0, Cannons.Length);
            GameObject bomb = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            bomb.transform.position = Cannons[pos].transform.position;
            bomb.transform.rotation = Cannons[pos].rotation;
            bomb.GetComponent<EnemyProjectile>().Damage = Damage;
            PlayWeaponFireSound();
        }
    }
}