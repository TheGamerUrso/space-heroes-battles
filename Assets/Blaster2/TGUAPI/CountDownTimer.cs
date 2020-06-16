
public class CountDownTimer
{
    public float m_CountdownTimer;
    public float delay;

    public CountDownTimer(float delay)
    {
        m_CountdownTimer = delay;
    }

    public bool countToZero()
    {
        if (m_CountdownTimer <= 0)
        {
            return false;
        }
        return true;
    }

    public float GetPowerUpCountdown()
    {
        return (100f / delay) * m_CountdownTimer;
    }
}