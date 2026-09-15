using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;

public class SlowMo : MonoBehaviour
{
    private bool IsActive { get; set; }
    private float delayTheSlowMoEffectTimer = .3f;
    private IAudioService audioService;
    private GameController gameController;

    private void Awake()
    {
        audioService = GameContext.Get<IAudioService>();
    }

    public void DoSlowMo()
    {
        switch (gameController.CurrentGameState)
        {
            case GameState.START:
                ResetTime();
                break;
            case GameState.GAME:

                if (Input.touchCount > 0 || Input.GetMouseButton(0))
                {
                    IsActive = false;
                }
                else
                {
                    IsActive = true;
                }

                if (IsActive)
                {
                    SlowTime();
                }
                else if (!IsActive && Time.timeScale < 1)
                {
                    ResetTime();
                }

                break;
            case GameState.GAMEOVER:
                ResetTime();
                break;
            case GameState.WIN:
                ResetTime();
                break;
            default:
                break;
        }
    }

    public void ResetTime()
    {
        Time.timeScale = 1.0f;
        audioService.SetPitch(1.0f);
    }
    public void SlowTime()
    {
        Time.timeScale = delayTheSlowMoEffectTimer;
        audioService.SetPitch(.8f);
    }

}
