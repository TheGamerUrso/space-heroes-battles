using System.Collections;
using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.UI;

public class LoadingController : ServiceComponent<ILoadingService>, ILoadingService
{
    [Header("LoadingScreen")]
    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private Image progressBar;

    [SerializeField] private CanvasGroup BlockRaycast;
    [SerializeField] private GameObject ProgressBarPanel;
    [SerializeField] private GameObject Content;
    [SerializeField] private GateControl[] Gates;

    [SerializeField] private SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader.OnSceneLoadStarted += Show;
        sceneLoader.OnSceneLoadEnded += Hide;

        Hide();
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
        BlockRaycast.blocksRaycasts = false;
        for (int i = 0; i < Gates.Length; i++)
        {
            GateControl gate = Gates[i];
            gate.OpenGate();
        }
    }
}
