using System.Collections;
using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.UI;

public class LoadingController : ServiceComponent<ILoadingService>, ILoadingService
{

    #region Loading Screen
    [Header("LoadingScreen")]
    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private Image progressBar;

    [SerializeField] private CanvasGroup BlockRaycast;
    [SerializeField] private GameObject ProgressBarPanel;
    [SerializeField] private GameObject Content;
    [SerializeField] private GateControl[] Gates;

    WaitForSeconds shortWait = new WaitForSeconds(2.0f);

    #endregion
    public void ToggleLoadingScreenCanvas(bool show)
    {
        LoadingScreen.GetComponent<Canvas>().enabled = show;
    }

    private IEnumerator ShowLoadingScreen(LevelEnum level, bool showLoadingScreen = true)
    {
        if (showLoadingScreen)
        {
            //LoadingScreen.GetComponent<Canvas>().enabled = true;
            Show();
            yield return new WaitForSeconds(.5f);
        }
        yield return shortWait;
        Content.SetActive(true);
       //StartCoroutine(LoadSceneAsync(level));
    }

    private void Show()
    {
        for (int i = 0; i < Gates.Length; i++)
        {
            GateControl gate = Gates[i];
            gate.CloseGate();
        }
    }

    private void Hide()
    {
        Content.SetActive(false);
        //BlockRaycast.blocksRaycasts = false;
        for (int i = 0; i < Gates.Length; i++)
        {
            GateControl gate = Gates[i];
            gate.OpenGate();
        }
    }

}
