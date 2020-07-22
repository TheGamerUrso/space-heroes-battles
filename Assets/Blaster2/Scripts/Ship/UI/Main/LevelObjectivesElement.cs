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

    bool complete = false;
    [SerializeField] private Image ProgressFill;
    AnimationCurve animationCurve; 

    public void SetLevelObjective(LevelObjectiveData levelObjectiveData)
    {
        this.levelObjectiveData = levelObjectiveData;
        RefreshLevelObjectiveEement();
    }

    private void Update()
    {
        if (complete)
        {
            ProgressFill.fillAmount = Mathf.Lerp(ProgressFill.fillAmount, animationCurve.Evaluate(Time.time), 1);
            if (ProgressFill.fillAmount == 1)
            {
                completedSprite.gameObject.SetActive(true);
            }
        }
    }

    public void MarkAsCompleted()
    {
        complete = true;
      
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