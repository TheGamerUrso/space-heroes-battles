using System.Collections;
using System.Linq;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class GrenadeProjectile : EnemyProjectile
{

    [SerializeField] private float duration = 2;

    public GameObject Ball;
    [SerializeField] private Transform[] Cannons;
    private bool exploded;
    [SerializeField] private WeaponData weaponData = null;
    private GameObject InstansiatedProjectile;

    private void Start()
    {
        Cannons = transform.GetChild(1).Cast<Transform>().ToArray();
    }

    private void OnEnable()
    {
        duration = 2;
        exploded = false;
        Ball.SetActive(true);
    }

    public override void Movement()
    {
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            rigid.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);

        }

        if (!exploded && duration <= 0)
        {
            StartCoroutine(Explode());
        }
    }

    public void Fire()
    {
        for (int i = 0; i < Cannons.Length; i++)
        {
            InstansiateProjectiles(i);
        }
    }

    private IEnumerator Explode()
    {
        exploded = true;
        yield return new WaitForSeconds(1.0f);
        Fire();
        Ball.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }

    private void InstansiateProjectiles(int i)
    {
        InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(weaponData.m_Projectile);

        InstansiatedProjectile.transform.SetPositionAndRotation(transform.position, Cannons[i].rotation);

        InstansiatedProjectile.GetComponent<EnemyProjectile>().setDamage(weaponData.m_WeaponDamage);

    }
}