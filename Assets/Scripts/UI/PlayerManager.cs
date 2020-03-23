using System;
using UnityEngine;

public class PlayerManager
{
    private static PlayerManager instance;
    private static PlayerManager Instance
    {
        get
        {
            return instance;
        }
    }

    private static GameObject currentPlayer;
    private static PlayerShipElement[] listOfPlayerShips;


    public PlayerManager()
    {
        if (instance == null)
        {
            instance = this;
        }

    }


    public static GameObject CreatePlayer(int id)
    {
        return CreatePlayerById(id);
    }

    public static GameObject CreatePlayerById(int id)
    {
        if (id >= listOfPlayerShips.Length)
        {
            id = 0;
        }

        currentPlayer = GameObject.Instantiate(listOfPlayerShips[id].prefab.gameObject);

        currentPlayer.SetActive(true);

        currentPlayer.GetComponent<PlayerShip>().SetShipStatSystem(listOfPlayerShips[id].prefab.GetShipStatsSystem());

        currentPlayer.GetComponent<PlayerShip>().SetLevelSystem(listOfPlayerShips[id].prefab.GetLevelSystem());

        currentPlayer.GetComponent<PlayerShip>().GetUpgradeSystem().SetPlayerData(DataController.GetPlayerData());


        return currentPlayer;

    }

    public static PlayerShipElement GetPlayerByID(int id)
    {
        return listOfPlayerShips[id];
    }

    public static void LoadPlayerSettings()
    {
        listOfPlayerShips = GameManager.Instance.ListOfPlayerShips();
        PlayerData playerData = DataController.GetPlayerData();
        for (int i = 0; i < listOfPlayerShips.Length; i++)
        {
            var Level = playerData.Level;
            var xp = playerData.xp;
            var xpToLevel = playerData.xpToLevel;
            listOfPlayerShips[i].prefab.SetLevelSystem(new LevelSystem(Level, xp, xpToLevel, 20));
            listOfPlayerShips[i].prefab.GetUpgradeSystem().SetPlayerData(DataController.GetPlayerData());
        }
    }

    public static PlayerShip GetPlayer()
    {
        if (currentPlayer == null)
        {
            return null;
        }
        return currentPlayer.GetComponent<PlayerShip>();
    }
}