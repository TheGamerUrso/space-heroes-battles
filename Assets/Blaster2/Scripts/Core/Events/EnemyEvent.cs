using UnityEngine;

public class EnemyEvent
{
    public enum EnemyEventType 
    {
        NONE,
        ENTER,
        ESCAPE,
        DEATH,
        HIT
    }

    public EnemyEventType Type;
    public Enemy Enemy;
    public float Value;

}
