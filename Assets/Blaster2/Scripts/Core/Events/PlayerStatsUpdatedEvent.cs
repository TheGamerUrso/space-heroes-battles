public class PlayerStatsUpdatedEvent 
{
    public enum StatType
    {
        None,
        Score,
        HighScore,
        Kills,
        SuperUsed,
        WaveSurvived,
        BountyKilled,
        EnemyKilled,
        ChargePower,
        PowerPackCollected,
        XP,
        EnemyEscaped
    }

    public StatType type;
    public float value;
}
