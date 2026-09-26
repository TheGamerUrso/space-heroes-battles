using System;

public interface IDamagable
{
    bool IsAlive { get; }
    float CurrentHealth { get; }
    void TakeDamage(float dmg);
    void Heal(float ammount);
}