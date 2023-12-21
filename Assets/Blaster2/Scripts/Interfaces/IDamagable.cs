using System;

public interface IDamagable
{
    void TakeDamage(float dmg);
    void Heal(float ammount);
    void Death();
}