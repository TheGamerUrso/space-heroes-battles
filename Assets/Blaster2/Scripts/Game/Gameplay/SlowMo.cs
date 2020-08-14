using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMo : MonoBehaviour
{
    private float delayTheSlowMoEffectTimer = .3f;
    private float delay = 4;

    void Update()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            return;
        }
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            Game.SlowMo = false;
        }
        else
        {
            Game.SlowMo = true;
        }

        DoSlowMo();
    }
    public void DoSlowMo()
    {
        if (GameController.CurrentGameState == GameController.GameState.GAME)
        {
            if (!Game.IsGameOver && !Game.IsPaused && Game.UseSlowMo)
            {
                if (Game.SlowMo)
                {
                    Time.timeScale = delayTheSlowMoEffectTimer;
                    AudioManager.Instance.SetPitch(.8f);
                }
                else if (!Game.SlowMo && Time.timeScale < 1)
                {
                    Time.timeScale = 1.0f;
                    AudioManager.Instance.SetPitch(1.0f);
                }
            }
        }
    }

}
