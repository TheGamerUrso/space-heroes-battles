using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : State
{
    public MenuState(GameManager gameManager) : base(gameManager)
    {
    }
    public override void OnStateEnter() {
        Application.targetFrameRate = 30;
    }
    public override void OnStateExit() { }

    public override void Tick()
    {
      
    }
}
