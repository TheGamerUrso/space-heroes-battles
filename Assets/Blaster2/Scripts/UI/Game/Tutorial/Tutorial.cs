using TheGamerUrso.Core;
using UnityEngine;

[System.Serializable]
public struct TutorialItem
{
    public string Name;
    public string Description;
    public GameObject prefabItem;
}

public class Tutorial : MonoSingleton<Tutorial>
{
    public UIView TutorialView;

    public TMPro.TextMeshProUGUI Title;
    public TMPro.TextMeshProUGUI Description;

    public TutorialItem[] TutorailItemsToShow;


    private void Start()
    {
        TutorialView.Hide();
    }

    public void ShowTutorial(int itemToShowIndex)
    {
        if (!GameController.Instance.IsGameOver)
        {
            var appService = GameContext.Get<IAppService>();
            appService.PauseTheGame(true);
            TutorialView.Show();
            for (int i = 0; i < TutorailItemsToShow.Length; i++)
            {
                TutorailItemsToShow[i].prefabItem.SetActive(false);

            }

            Title.text = TutorailItemsToShow[itemToShowIndex].Name;
            Description.text = TutorailItemsToShow[itemToShowIndex].Description;
            TutorailItemsToShow[itemToShowIndex].prefabItem.SetActive(true);

            Time.timeScale = 0;
        }
    }

    public void Close()
    {
        var playerService = GameContext.Get<PlayerManager>();
        var appService = GameContext.Get<IAppService>();
        TutorialView.Hide();
        appService.PauseTheGame(false);
        Time.timeScale = 1;
        playerService.GetPlayer().tempGodMode();
    }

}
