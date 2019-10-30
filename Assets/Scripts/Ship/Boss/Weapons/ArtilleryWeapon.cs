using System.Collections;
using UnityEngine;

public class ArtilleryWeapon : WeaponScript
{
    [Header("Artillery")]
    public GameObject TargetPrefab;
    public Transform[] artilleryTargets;
    private GameObject tarGO;
    public GameObject BulletPrefab;

    public override void Initialize()
    {
        base.Initialize();

        tarGO = GameObject.Find(TargetPrefab.name);

        if (tarGO == null)
        {
            tarGO = Instantiate(TargetPrefab, new Vector3(0, -50, 0), Quaternion.identity);
            tarGO.name = TargetPrefab.name;
        }

        artilleryTargets = new Transform[tarGO.transform.childCount];

        for (int i = 0; i < tarGO.transform.childCount; i++)
        {
            artilleryTargets[i] = tarGO.transform.GetChild(i);
        }

        StartCoroutine(ShootDelay());
    }

    public override void Shoot()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);

            bullet.GetComponent<ArtilleryProjectile>().Targets(artilleryTargets);
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
                GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
                ArtilleryProjectile artilleryProjectile = bullet.GetComponent<ArtilleryProjectile>();
                artilleryProjectile.Targets(artilleryTargets);
                artilleryProjectile.setDamage(weaponData.m_WeaponDamage);
                yield return new WaitForSeconds(randomDelay);
            }
            delayBetweenShots = UnityEngine.Random.Range(4, 6);
        }
    }
}