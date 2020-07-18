using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "BLASTER2/Create New Weapon",order =1)]
public class Weapon_SO : ScriptableObject
{
    public string Name;
    public PoolGameObjectType m_Projectile;
    public float FireRate;
    public float Damage;
    public float SuperChargeTime;

    public AudioClip ShootSFX;
    public int Radius;
    public int Angle;
    public float delayBetweenShots;
}