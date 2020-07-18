using System.Collections;
using UnityEngine;

public class GrenadeProjectile : EnemyProjectile
{

    [SerializeField] private float duration = 2;
    public float startDuration;

    public GameObject Ball;
    private bool exploded;
    [SerializeField] private Weapon_SO weaponData = null;
    private GameObject InstansiatedProjectile;

    private Vector3[] pos;
    private void Start()
    {
        pos = new Vector3[]{
            new Vector3(0,0,0),
         new Vector3(0,20,0),
         new Vector3(0,40,0),
         new Vector3(0,60,0),
         new Vector3(0,80,0),
         new Vector3(0,100,0),
         new Vector3(0,120,0),
         new Vector3(0,140,0),
         new Vector3(0,160,0),
         new Vector3(0,180,0),
         new Vector3(0,200,0),
         new Vector3(0,220,0),
            new Vector3(0,240,0), new Vector3(0,260,0), new Vector3(0,280,0), new Vector3(0,300,0)
        , new Vector3(0,320,0), new Vector3(0,340,0), new Vector3(0,360,0)};
    }

    protected override void OnEnable()
    {
        duration = startDuration;
        exploded = false;
        Ball.SetActive(true);
    }

    public override void Movement()
    {
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            base.Movement();
        }

        if (!exploded && duration <= 0)
        {
            StartCoroutine(Explode());
        }
    }

    public void Fire()
    {
        for (int i = 0; i < pos.Length; i++)
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

        InstansiatedProjectile.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(pos[i]));

        InstansiatedProjectile.GetComponent<EnemyProjectile>().Damage = weaponData.Damage;

    }
}