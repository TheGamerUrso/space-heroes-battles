using System;

public interface IDamagable
{
    event Action<float, float> OnHealthChanged;
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
    void TakeDamage(float dmg);
    void Heal(float ammount);
    void Death();
}