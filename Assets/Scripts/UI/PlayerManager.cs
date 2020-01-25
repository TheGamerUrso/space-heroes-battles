using System;
using UnityEngine;


public static class PlayerManager
{

    private static GameObject currentPlayer;
    private static PlayerShip[] listOfPlayerShips;

    public static void Initialize(PlayerShip[] newListOfPlayerShips)
    {
        listOfPlayerShips = newListOfPlayerShips;
        LoadPlayerSettings();    
    }

    public static void CreatePlayer(int id)
    {
        CreatePlayerById(id);
    }

    public static void CreatePlayerById(int id)
    {
        if (id >= listOfPlayerShips.Length)
        {
            id = 0;
        }
        GameObject holder = GameObject.Find("DynamicObjects");
        currentPlayer = GameObject.Instantiate(listOfPlayerShips[id].prefab.gameObject, holder.transform,false);

        currentPlayer.SetActive(true);

        currentPlayer.GetComponent<Player>().SetShipStatSystem(listOfPlayerShips[id].prefab.GetShipStatsSystem());

        currentPlayer.GetComponent<Player>().SetLevelSystem(listOfPlayerShips[id].prefab.GetLevelSystem());

        currentPlayer.GetComponent<Player>().GetUpgradeSystem().SetPlayerData(DataController.GetPlayerData());

      
    }

    public static PlayerShip GetPlayerByID(int id)
    {
        return listOfPlayerShips[id];
    }

    public static void LoadPlayerSettings()
    {
        PlayerData playerData = DataController.GetPlayerData();
        for (int i = 0; i < listOfPlayerShips.Length; i++)
        {
            var Level = playerData.Level;
            var xp = playerData.xp;
            var xpToLevel = playerData.xpToLevel;
            listOfPlayerShips[i].prefab.SetLevelSystem(new LevelSystem(Level, xp, xpToLevel,20));
            listOfPlayerShips[i].prefab.GetUpgradeSystem().SetPlayerData(DataController.GetPlayerData());
        }
    }

    public static Player GetPlayer()
    {
        if (currentPlayer == null)
        {
            int shipSelected = GameManager.CurrentHeroChoosen;
            CreatePlayerById(shipSelected);
        }
        return currentPlayer.GetComponent<Player>();
    }
}