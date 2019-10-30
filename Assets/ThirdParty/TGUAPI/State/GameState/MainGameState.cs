using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameState : State
{
    public MainGameState(GameManager gameManager) : base(gameManager)
    {
    }

    public override void OnStateEnter() {

        PlayerData playerData = DataController.GetPlayerData();

        playerData.GotHitInGame = false;
        playerData.PlayedGame = false;

        AudioManager.PlayRandomMusic(true);

        Application.targetFrameRate = 60;
    }

    public override void OnStateExit() { System.GC.Collect(); }

    public override void Tick()
    {
       
    }
}
