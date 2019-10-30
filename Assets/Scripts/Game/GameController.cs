    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class GameController : MonoBehaviour
{
    public GameStates gameState;
    public GameObject[] Essentials;

    void Awake()
    {
        if (GameObject.FindObjectOfType<GameManager>())
        {
            return;
        }

        for (int i = 0; i < Essentials.Length; i++)
        {

            GameObject essentialGO = Instantiate(Essentials[i]) as GameObject ;
            essentialGO.name = Essentials[i].name;
        }

        GameManager.instance.SetState(gameState);
    }

}
