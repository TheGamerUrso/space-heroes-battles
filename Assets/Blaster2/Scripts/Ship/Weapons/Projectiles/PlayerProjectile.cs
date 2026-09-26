using UnityEngine;

public class PlayerProjectile : BaseProjectile
{
    public Material[] playerMaterials;

    public override void SetOwner(BaseWeapon baseWeapon)
    {
        this.baseWeapon = baseWeapon;
    }

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
}