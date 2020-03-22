using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TutorialItem
{
    public string Name;
    public string Description;
    public GameObject prefabItem;
}

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance;

    public GameObject TutorialWindow;

    public TMPro.TextMeshProUGUI Title;
    public TMPro.TextMeshProUGUI Description;

    public TutorialItem[] TutorailItemsToShow;
    private float delay = 1.0f;


    public void ShowTutorial(int itemToShowIndex)
    {
        for (int i = 0; i < TutorailItemsToShow.Length; i++)
        {
            TutorailItemsToShow[i].prefabItem.SetActive(false);
        }

        Title.text = TutorailItemsToShow[itemToShowIndex].Name;
        Description.text = TutorailItemsToShow[itemToShowIndex].Description;
        TutorailItemsToShow[itemToShowIndex].prefabItem.SetActive(true);

        TutorialWindow.SetActive(true);
        Time.timeScale = 0;
    }

    public void Close()
    {
        TutorialWindow.SetActive(false);
        Time.timeScale = 1;
        PlayerManager.GetPlayer().tempGodMode();
    }




    void Start()
    {
        Instance = this;
    }


}
