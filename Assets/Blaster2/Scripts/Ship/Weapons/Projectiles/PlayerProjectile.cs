using UnityEngine;

public class PlayerProjectile : BaseProjectile
{
    public Material[] playerMaterials;

    protected override void Awake()
    {
        base.Awake();
        PlayerData playerData = dataService.GetPlayerData();
        meshRenderer.material = playerMaterials[playerData.CurrrentSelectedShip];
    }

    public override void Movement()
    {
        transform.position += shootDir * speed * Time.deltaTime;

        if (transform.position.z > Constants.m_ZMax)
        {
            gameObject.SetActive(false);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var destroyable = other.GetComponent<IDamagable>();
            if (destroyable != null)
            {
                destroyable.TakeDamage(Damage);
            }
            DestoryNow();
        }
    }

    [ContextMenu("Debug_Projectile")]
    public void Debug_SpawnEnemyElement()
    {
       
        transform.SetPositionAndRotation(new Vector3(0, 0, 0),
            Quaternion.Euler(new Vector3(0, 0, 0)));

        SetShootDir(transform.forward);
        gameObject.SetActive(true);
    }
}