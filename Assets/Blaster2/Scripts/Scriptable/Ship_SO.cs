using UnityEngine;

public abstract class Ship_SO : ScriptableObject
{
    public float baseHealth;
    [Range(10, 140)]
    public float baseSpeed;
    public float baseFireRate;
    public float baseDamage;
    public int EnemyValue;
    public PoolGameObjectType ExplostionEffect;
}
