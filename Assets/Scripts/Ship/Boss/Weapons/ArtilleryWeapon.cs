using System.Collections;
using UnityEngine;

public class ArtilleryWeapon : WeaponScript
{
    [Header("Artillery")]
    public GameObject TargetPrefab;
    public Vector3[] Positions;
    public Transform[] artilleryTargets;
    private GameObject tarGO;
    public GameObject BulletPrefab;

    public override void Initialize()
    {
        base.Initialize();

        StartCoroutine(ShootDelay());
    }

    public override void Shoot()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator ShootDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenShots);
            float randomDelay = UnityEngine.Random.Range(.4f, 1);
            for (int i = 0; i < Random.Range(2, 4); i++)
            {
                GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.Euler(0,0,0));
                ArtilleryProjectile artilleryProjectile = bullet.GetComponent<ArtilleryProjectile>();
                artilleryProjectile.Damage = Damage;
                yield return new WaitForSeconds(randomDelay);
            }
            delayBetweenShots = UnityEngine.Random.Range(4, 6);
        }
    }
}