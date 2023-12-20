using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMo : MonoBehaviour
{
    private float delayTheSlowMoEffectTimer = .3f;
    private float delay = 4;

    void Update()
    {

        DoSlowMo();
    }

    public bool SlowMoAvailable()
    {
        return !Game.IsGameOver && !Game.IsPaused && Game.UseSlowMo;
    }

    public void DoSlowMo()
    {
        switch (GameController.CurrentGameState)
        {
            case GameController.GameState.START:
                ResetTime();
                break;
            case GameController.GameState.GAME:

                if (Input.touchCount > 0 || Input.GetMouseButton(0))
                {
                    Game.SlowMo = false;
                }
                else
                {
                    Game.SlowMo = true;
                }

                if (SlowMoAvailable())
                {
                    if (Game.SlowMo)
                    {
                        SlowTime();
                    }
                    else if (!Game.SlowMo && Time.timeScale < 1)
                    {
                        ResetTime();
                    }
                }
                break;
            case GameController.GameState.GAMEOVER:
                ResetTime();
                break;
            case GameController.GameState.WIN:
                ResetTime();
                break;
            default:
                break;
        }
    }

    public void ResetTime()
    {
        Time.timeScale = 1.0f;
        AudioManager.Instance.SetPitch(1.0f);
    }
    public void SlowTime()
    {
        Time.timeScale = delayTheSlowMoEffectTimer;
        AudioManager.Instance.SetPitch(.8f);
    }

}
