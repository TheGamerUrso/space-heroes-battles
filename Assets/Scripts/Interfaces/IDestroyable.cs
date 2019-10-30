public interface IDestroyable
{
    bool IsAlive { get; set; }
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
    void TakeDamage(float dmg);
}