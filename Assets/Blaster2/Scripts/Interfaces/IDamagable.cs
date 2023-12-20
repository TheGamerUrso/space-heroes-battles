using System;

public interface IDamagable
{
    float MaxHealth { get;}
    float CurrentHealth { get;}
    void TakeDamage(float dmg);
    void Heal(float ammount);
    void Death();
}