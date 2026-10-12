using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonAudio : MonoBehaviour
{
    public IAudioService audioService;
    public Button button;
    public string Id = "UI_Click";

    void Start()
    {
        audioService = GameContext.Get<IAudioService>();

        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlaySound);
        }
    }

    public void PlaySound()
    {
        audioService.PlaySound(Id);
    }
}
