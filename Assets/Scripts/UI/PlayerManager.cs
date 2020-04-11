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


    public PlayerManager(GameManager gm,GameManager dc)
    {
        if (instance == null)
        {
            this.gm = gm;
            this.dc = dc;
            instance = this;
        }

    }

    private GameManager gm;
    private GameManager dc;

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

        PlayerData playerData = GameManager.Instance.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();

        int level = playerShipData.level;
        float xp = playerShipData.xp;
        float xpToLevel = playerShipData.xpToLevel;

        currentPlayer.GetComponent<PlayerShip>().level = level;
        currentPlayer.GetComponent<PlayerShip>().xp = xp;
        currentPlayer.GetComponent<PlayerShip>().xpToLevel = xpToLevel;

        currentPlayer.GetComponent<PlayerShip>().SetStats(level);

        currentPlayer.GetComponent<PlayerShip>().SetPlayerData(playerData);


        return currentPlayer;

    }

    public static PlayerShipElement GetPlayerByID(int id)
    {
        return listOfPlayerShips[id];
    }

    public void LoadPlayerSettings()
    {
        listOfPlayerShips = gm.ListOfPlayerShips();

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