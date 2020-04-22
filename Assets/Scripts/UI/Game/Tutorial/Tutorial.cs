using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[System.Serializable]
public class TutorialItem
{
    public string Name;
    public string Description;
    public GameObject prefabItem;
}

public class Tutorial : Singleton<Tutorial>
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowTutorial(0);
        }
    }

    public void Close()
    {
        TutorialView.Hide();
        Time.timeScale = 1;
        PlayerManager.GetPlayer().tempGodMode();
    }

}
