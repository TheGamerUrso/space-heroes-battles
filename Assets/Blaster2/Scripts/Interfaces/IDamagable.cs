using System;

public interface IDamagable
{
    event Action<float, float> OnHealthChanged;
    float MaxHealth { get;}
    float CurrentHealth { get;}
    void TakeDamage(float dmg);
    void Heal(float ammount);
    void Death();
}