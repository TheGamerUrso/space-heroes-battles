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
    [SerializeField] private AnimationCurve animationCurve;

    [SerializeField] private float speed = 1;
    private float progressTimer = 0;

    public void SetLevelObjective(LevelObjectiveData levelObjectiveData)
    {
        this.levelObjectiveData = levelObjectiveData;
        completedSprite.gameObject.SetActive(false);
        failSpriet.gameObject.SetActive(false);

        ID = GameManager.LevelIndexSelected;
        DescriptionText.text = levelObjectiveData.description;

        if (levelObjectiveData.completed)
        {
            ProgressFill.fillAmount = 1;
            completedSprite.gameObject.SetActive(true);
        }
        else if (!levelObjectiveData.completed)
        {
            ProgressFill.fillAmount = 0;
        }
    }

    private void Update()
    {
        if (complete)
        {
            progressTimer += Time.deltaTime * speed;
            ProgressFill.fillAmount = Mathf.Lerp(ProgressFill.fillAmount, animationCurve.Evaluate(progressTimer), 1);
            if (ProgressFill.fillAmount >= 1)
            {
                completedSprite.gameObject.SetActive(true);
                complete = false;
            }
        }
    }

    public void MarkAsCompleted()
    {
        complete = true;
        progressTimer = 0;
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
}