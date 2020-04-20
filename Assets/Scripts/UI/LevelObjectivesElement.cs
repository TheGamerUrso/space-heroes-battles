using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelObjectivesElement : MonoBehaviour
{
    public int ID;
    public TextMeshProUGUI DescriptionText;
    public LevelObjectiveData levelObjectiveData;
    public Image completedSprite;
    public Image failSpriet;

    public void MarkAsCompleted()
    {
        completedSprite.gameObject.SetActive(true);
    }
    public void MarkAsFailed()
    {

        failSpriet.gameObject.SetActive(true);
    }
    public void CheckComplete()
    {
        if (levelObjectiveData.completed)
        {
            MarkAsCompleted();
        }
        else if (!levelObjectiveData.completed)
        {
            MarkAsFailed();
        }
    }

    public void RefreshLevelObjectiveEement()
    {
        completedSprite.gameObject.SetActive(false);
        failSpriet.gameObject.SetActive(false);

        ID                      = GameManager.LevelIndexSelected;
        DescriptionText.text    = levelObjectiveData.description;

        if (levelObjectiveData.completed)
        {
            MarkAsCompleted();
        }
    }
}