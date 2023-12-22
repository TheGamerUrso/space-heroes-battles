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
        return !GameController.Instance.IsGameOver && !GameManager.Instance.IsPaused && GameController.Instance.UseSlowMo;
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
                    GameController.Instance.SlowMo = false;
                }
                else
                {
                    GameController.Instance.SlowMo = true;
                }

                if (SlowMoAvailable())
                {
                    if (GameController.Instance.SlowMo)
                    {
                        SlowTime();
                    }
                    else if (!GameController.Instance.SlowMo && Time.timeScale < 1)
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
