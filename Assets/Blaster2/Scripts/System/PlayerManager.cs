using System;
using UnityEngine;
[Serializable]
public struct PlayerShipElement
{
    public string name;
    public GameObject prefab;
}


public class PlayerManager : MonoBehaviour
{
    private GameObject currentPlayer;
    [SerializeField] private PlayerShipElement[] PlayerShips;

    public GameObject CreatePlayer(int id)
    {
        return CreatePlayerById(id);
    }

    public GameObject CreatePlayerById(int id)
    {
        if (id >= PlayerShips.Length)
        {
            id = 0;
        }

        currentPlayer = GameObject.Instantiate(PlayerShips[id].prefab.gameObject);

        currentPlayer.SetActive(true);

        return currentPlayer;
    }

    public PlayerShipElement GetPlayerByID(int id)
    {
        return PlayerShips[id];
    }

    public PlayerShip GetPlayer()
    {
        if (currentPlayer == null)
        {
            return null;
        }
        return currentPlayer.GetComponentInChildren<PlayerShip>();
    }
}