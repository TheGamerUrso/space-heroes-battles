using System;

public interface IDestroyable
{
    Action<float, float> OnHealthChange { get; set; }
    bool IsDestroyed { get; set; }
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
    void TakeDamage(float dmg);
}