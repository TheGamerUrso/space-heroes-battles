using TheGamerUrso.PoolSystem;
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

    [Header("Rocket Launcher")]
    public bool m_HomeMissleUpgrade;

    public bool m_RocketUpgrade;
    public int m_NumberOfMissiles = 0;

    public bool RapidFireMode;
    public bool SuperRockFireMode;

    public bool SummonTurrets;
}