using System.Collections;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalGameOverScreen : MonoBehaviour
{
    private IAppService appService;

    private void Awake()
    {
        appService = GameContext.Get<IAppService>();
    }
    public void QuitButton()
    {
        appService.LoadMainMenu();
        GetComponent<CanvasGroup>().interactable = false;
    }

    public void ReplayButton()
    {
        appService.ResetLevel();
        GetComponent<CanvasGroup>().interactable = false;
    }
}
