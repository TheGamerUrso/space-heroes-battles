using System;
using UnityEngine;


public class PlayerManager{
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerManager(new PlayerShip[0]);
            }
            return instance;
        }
    }


    private GameObject currentPlayer;
    private PlayerShip[] players;

    public PlayerManager(PlayerShip[] players)
    {
        instance = this;
        this.players = players;
        LoadPlayerSettings();
       
    }

    public static void CreatePlayer(int id)
    {
        instance.CreatePlayerById(id);
    }

    public void CreatePlayerById(int id)
    {
        if (id >= players.Length)
        {
            id = 0;
        }

        currentPlayer = GameObject.Instantiate(players[id].prefab.gameObject);

        currentPlayer.SetActive(true);

        currentPlayer.GetComponent<Player>().SetShipStatSystem(players[id].prefab.GetShipStatsSystem());

        currentPlayer.GetComponent<Player>().SetLevelSystem(players[id].prefab.GetLevelSystem());

        currentPlayer.GetComponent<Player>().GetUpgradeSystem().SetPlayerData(DataController.GetPlayerData());

      
    }

    public PlayerShip GetPlayerByID(int id)
    {
        return players[id];
    }

    public void LoadPlayerSettings()
    {
        PlayerData playerData = DataController.GetPlayerData();
        for (int i = 0; i < players.Length; i++)
        {
            var Level = playerData.Level;
            var xp = playerData.xp;
            var xpToLevel = playerData.xpToLevel;
            players[i].prefab.SetLevelSystem(new LevelSystem(Level, xp, xpToLevel,20));
            players[i].prefab.GetUpgradeSystem().SetPlayerData(DataController.GetPlayerData());
        }
    }

    public static Player GetPlayer()
    {
        if (instance.currentPlayer == null)
        {
            int shipSelected = GameManager.CurrentHeroChoosen;
            instance.CreatePlayerById(shipSelected);
        }
        return Instance.currentPlayer.GetComponent<Player>();
    }
}