using System;

public interface IDamagable
{
    bool IsAlive { get; }
    float CurrentHealth { get; }
    void TakeDamage(float dmg,bool IgnoreShield = false);
    void Heal(float ammount);
}