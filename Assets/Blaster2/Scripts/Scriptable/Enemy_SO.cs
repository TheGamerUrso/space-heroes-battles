using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "Enemy", menuName = "New Enemy")]
public class Enemy_SO : ScriptableObject
{
    public float baseHealth;

    public float baseSpeed;
    public float baseFireRate;
    public float baseDamage;

    public int EnemyValue;

    public HealthBarSettings HealthBarSettings;

    public AudioClip hitSFX;

    public PoolGameObjectType ExplostionEffect;
}
