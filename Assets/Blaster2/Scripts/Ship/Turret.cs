using System;
using UnityEngine;

public class Turret : Ship
{
    [SerializeField] private BaseWeapon baseWeapon;
    protected float hitEffectTimer;
    [SerializeField] protected GameObject hitEffect;

    public ShipData shipData;
    public Ship_SO turrentData;

    public override ShipData ShipData => shipData;

    public override Ship_SO Ship_SO => turrentData;

    private void Start()
    {
        healthComponent.OnHealthChanged -= OnHealthValueChanged;
    }
    //=================================================================================
    public void ExitLevel() => Destroy(gameObject);
    //=================================================================================
    public override void SetStats(int level)
    {
        shipData.Level = Mathf.Clamp(level, 1, 10);

        shipData.Health = level * Ship_SO.baseHealth;
        shipData.Speed = Ship_SO.baseSpeed;
        shipData.Damage = (level * Ship_SO.baseDamage);
        shipData.FireRate = Ship_SO.baseFireRate;

        healthComponent.Setup(this);
        weaponController.Setup(this);
        movementController.Setup(this);
    }
    //=================================================================================
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.ENEMYTAG))
        {
            healthComponent.TakeDamage(1);
        }
        if (other.tag.Equals(Constants.ENEMYPROJECTILETAG))
        {
            healthComponent.TakeDamage(1);
        }
    }
    //=================================================================================
    private void OnHealthValueChanged(float currentHealth, float maxHealth)
    {
        var healthPresentage = currentHealth / maxHealth;

        if (currentHealth < 1)
            Death();
        if (shipData.HasShield)
            DeactivateShield();

        Hit();
    }
    //=================================================================================
    public override void Death()
    {
        gameObject.SetActive(false);
    }
    //=================================================================================
    public override void Hit()
    {
        hitEffect.SetActive(true);
        hitEffectTimer = .5f;
    }
}