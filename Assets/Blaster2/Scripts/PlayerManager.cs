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


    public PlayerManager(GameManager gm, GameManager dc)
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

        currentPlayer = GameObject.Find("Player1Prefab");

        if (currentPlayer == null)
        {

            currentPlayer = GameObject.Instantiate(listOfPlayerShips[id].prefab.gameObject);

            currentPlayer.SetActive(true);
        }

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
        return currentPlayer.GetComponentInChildren<PlayerShip>();
    }
}