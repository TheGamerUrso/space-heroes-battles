using UnityEngine;

public interface IGameService
{
    BaseGameMode GetGameMode();
    void IncreaseMultiplier();
    void SetPlayerXP(float xPEarned);
    void SetScore(int enemyValue);
}
