using UnityEngine;

public interface IDataService 
{
    PlayerData GetPlayerData();
    void ReplacePlayerData(PlayerData playerData);
    void Load();
    void Save();

    Sprite GetAchievementIcon(int id);
}
