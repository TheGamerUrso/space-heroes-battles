using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelObjectivesElement : MonoBehaviour
{
    [SerializeField] private int ID;
    [SerializeField] private TextMeshProUGUI DescriptionText;
    [SerializeField] private LevelObjectiveData levelObjectiveData;
    [SerializeField] private Image completedSprite;
    [SerializeField] private Image failSpriet;

    public void SetLevelObjective(LevelObjectiveData levelObjectiveData)
    {
        this.levelObjectiveData = levelObjectiveData;
        RefreshLevelObjectiveEement();
    }


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

        ID = GameManager.LevelIndexSelected;
        DescriptionText.text = levelObjectiveData.description;

        if (levelObjectiveData.completed)
        {
            MarkAsCompleted();
        }
    }
}