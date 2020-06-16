[System.Serializable]
public class ProfileProfile
{
    public int Level;
    public int currentXp;
    public int xpToLevel;
    public int m_Coins;
    public int m_HighScore;
    public int m_EnemyKilledOverall = 0;

    public ProfileProfile()
    {
        Level = 1;
        currentXp = 0;
        xpToLevel = 100;
        
}
}
