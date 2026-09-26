using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuOptionUI : BaseOptions
{
    protected IAppService appService;
    public override void Start()
    {
        base.Start();
        appService = GameContext.Get<IAppService>();
    }
    public override void ExitAndSave()
    {
        base.ExitAndSave();
        appService.PauseTheGame(false);
    }

    public void Quit()
    {
        base.ExitAndSave();
        Time.fixedDeltaTime = 0.02f;
        appService.LoadMainMenu();
    }
}
