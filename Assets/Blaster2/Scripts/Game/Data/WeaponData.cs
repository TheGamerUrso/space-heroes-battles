using UnityEngine;

[CreateAssetMenu(menuName = "Create New Weapon")]
public class WeaponData : ScriptableObject
{
    public AudioClip ShootSoundEffect;
    public PoolGameObjectType m_Projectile;
    public bool Split;

    public bool AutoAttack;

    public float m_WeaponDamage;
    public bool CanAttack;
    public int multiplier;
}