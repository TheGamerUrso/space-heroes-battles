using UnityEngine;

public class ArtilleryWeapon : BaseWeapon
{
    private float cooldown;
    private int numberOfAttacks;
    private bool attack;

    public override void Update()
    {
        base.Update();

        if (!attack)
        {
            cooldown -= Time.deltaTime;
        }

        if (cooldown <= 0 && !attack)
        {
            cooldown = 2;
            numberOfAttacks = 0;
            attack = true;
        }

        if (numberOfAttacks > 3)
        {
            attack = false;
        }
    }


    public override void Shoot()
    {
        if (Time.time > newShot && AutoAttack && attack)
        {
            newShot = Time.time + FireRate;

            PlayWeaponFireSound();

            for (int i = 0; i < Cannons.Length; i++)
            {
                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
                dir = Cannons[i].position + Cannons[i].up;
                shootDir = (dir - Cannons[i].position).normalized;

                InstansiatedProjectile.SetActive(true);

                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(shootDir);

                EnemyProjectile enemyProjectile = InstansiatedProjectile.GetComponent<EnemyProjectile>();
               //enemyProjectile.Setup(this);
                enemyProjectile.SetShootDir(shootDir);
            }

            numberOfAttacks++;
        }
    }
}

