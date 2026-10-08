using TheGamerUrso.Core;
using UnityEngine;


public class PauseMenuOptionUI : BaseOptions
{
    protected IGUIService guiService;

    public override void Start()
    {
        base.Start();
        guiService = GameContext.Get<IGUIService>();
    }
    public override void ExitAndSave()
    {
        base.ExitAndSave();
        guiService.PauseButton();
    }

    public void Quit()
    {
        base.ExitAndSave();
        guiService.QuitButton();
    }
}
