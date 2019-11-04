public interface IDestroyable
{
    bool IsDestroyed { get; set; }
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
    void TakeDamage(float dmg);
}