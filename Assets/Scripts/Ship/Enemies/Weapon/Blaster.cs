using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class Blaster : WeaponScript
{
    private bool shooting;
    public bool delay;
    public bool Multiple;
    public float delayTimer;
    public int m_NumberOfBullets;
    public bool RandomRotationOnY;


    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Shoot()
    {
        if (ship == null)
        {
            ship = GetComponentInParent<Ship>();
        }

        if (shipStatsSystem == null)
        {
            return;
        }

        if (delay && !shooting)
        {
            StartCoroutine(ShowWitHDelay());
        }
        else if (Multiple & !shooting)
        {
            StartCoroutine(ShootRandomNumberOfProjectile());
        }
        else if(!delay && !Multiple)
        {
            Fire();
        }
    }

    public override void Fire()
    {
        if (Time.time > newShot)
        {
            newShot = Time.time + FireRate;

            PlayWeaponFireSound();

            for (int i = 0; i < Cannons.Length; i++)
            {
                InstansiateProjectiles(i);
            }
        }
    }
    private IEnumerator ShowWitHDelay()
    {
        shooting = true;

        for (int i = 0; i < Cannons.Length; i++)
        {
            InstansiateProjectiles(i);
            yield return new WaitForSeconds(delayBetweenShots);
        }

        yield return new WaitForSeconds(FireRate);
        shooting = false;
    }

    private IEnumerator ShootRandomNumberOfProjectile()
    {
        shooting = true;

        if (m_NumberOfBullets == 0)
        {
            m_NumberOfBullets = UnityEngine.Random.Range(8, 16);
        }

        List<Transform> templist = Cannons.ToList();
        Transform cannon;

        for (int i = 0; i < m_NumberOfBullets; i++)
        {
            GameObject newBullet = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);

            cannon = templist[UnityEngine.Random.Range(0, templist.Count)].transform;
            //templist.Remove(cannon);
            newBullet.transform.position = cannon.position;


            newBullet.transform.rotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(-25, 25), 0));
            //bomb.GetComponent<EnemyProjectile>().m_FollowPlayer =    weaponData.m_ShootDirectlyToPlayerPosition;
            newBullet.GetComponent<EnemyProjectile>().setDamage(Damage);
            //newBullet.transform.TransformDirection(direction);
            PlayWeaponFireSound();
            yield return new WaitForSeconds(delayBetweenShots);

        }

        yield return new WaitForSeconds(FireRate);
        m_NumberOfBullets = 0;
        shooting = false;
    }

    private void InstansiateProjectiles(int i)
    {

        InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
        if (Multiple)
        {
            InstansiatedProjectile.transform.position = transform.position;
            InstansiatedProjectile.transform.rotation = Quaternion.identity;
        }
        else
        {
            InstansiatedProjectile.transform.position = Cannons[i].position;
        }

        if (FollowRotation)
        {
            InstansiatedProjectile.transform.rotation = Cannons[i].rotation;
        }

        if (RandomRotationOnY)
        {
            InstansiatedProjectile.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(-40, 40), 0));
        }

        InstansiatedProjectile.GetComponent<EnemyProjectile>().setDamage(Damage);

    }
}