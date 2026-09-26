using UnityEngine;

public interface IDataService 
{
    PlayerData GetPlayerData();
    void Load();
    void Save();
}
