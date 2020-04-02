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
        int level = listOfPlayerShips[id].prefab.level;
        float xp = listOfPlayerShips[id].prefab.xp;
        float xpToLevel = listOfPlayerShips[id].prefab.xpToLevel;

        currentPlayer.GetComponent<PlayerShip>().level = level;
        currentPlayer.GetComponent<PlayerShip>().xp = xp;
        currentPlayer.GetComponent<PlayerShip>().xpToLevel = xpToLevel;

        currentPlayer.GetComponent<PlayerShip>().SetStats(level);

        currentPlayer.GetComponent<PlayerShip>().SetPlayerData(DataController.GetPlayerData());


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
            listOfPlayerShips[i].prefab.level = Level;
            listOfPlayerShips[i].prefab.xp = xp;
            listOfPlayerShips[i].prefab.xpToLevel = xpToLevel;
            listOfPlayerShips[i].prefab.MaxLevel = 20;


            listOfPlayerShips[i].prefab.SetPlayerData(DataController.GetPlayerData());
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