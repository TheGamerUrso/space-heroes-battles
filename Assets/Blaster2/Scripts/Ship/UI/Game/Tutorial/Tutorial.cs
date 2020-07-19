using Doozy.Engine.UI;
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

    protected override void Awake()
    {
        base.Awake();
    }


    private void Start()
    {
        TutorialView.Hide();
    }

    public void ShowTutorial(int itemToShowIndex)
    {
        Game.IsPaused = true;
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

    public void Close()
    {
        TutorialView.Hide();
        Game.IsPaused = false;
        Time.timeScale = 1;
        PlayerManager.GetPlayer().tempGodMode();
    }

}
