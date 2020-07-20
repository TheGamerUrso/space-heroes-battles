using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Level
{
    public string ID;
    public Mission mission;
    public int spriteId;
    public bool interactable;
    public bool Locked;

    public Level(string ID, Mission mission, int spriteId, bool interactable, bool Locked)
    {
        this.ID = ID;
        this.mission = mission;
        this.spriteId = spriteId;
        this.interactable = interactable;
        this.Locked = Locked;
    }
}

public class LevelElement : MonoBehaviour
{
    private Level level;

    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color LockedColor;
    [SerializeField] private GameObject lockedImage;

    private void OnEnable()
    {
        UpdateLevels();
    }

    public void SetLevelElement(Level level)
    {
        if (level == null)
        {
            SetEmptyLevelElement();
            return;
        }

        this.level = level;

        if (level.ID.Contains("Mission") || level.ID.Contains("Prologue"))
        {
            NameText.text = level.mission.Title + "\n" + "Level " + level.mission.Level + " Recomented";
        }
        else if (level.ID.Contains("Survival"))
        {
            NameText.text = "SurvivalMode";
        }

        UpdateLevels();
    }

    public void SelectLevel()
    {
        if (level.ID.Contains("Survival"))
        {
            ScreenManager.Instance.Open("LevelDetailScreen");
            GameManager.LevelIndexSelected = 0;
        }
        else
        {
            GameManager.Instance.SetMission(level.mission);
            ScreenManager.Instance.Open("LevelDetailScreen");
        }
        LevelDetailScreen.Instance.ShowDetailScreen();
    }

    public void UpdateLevels()
    {
        if(level == null)
        {
            return;
        }

        button.interactable = level.interactable;

        if (level.Locked)
        {
            NameText.color = LockedColor;
            buttonImage.color = LockedColor;
            lockedImage.SetActive(true);
        }
        else if (!level.Locked)
        {
            NameText.color = normalColor;
            buttonImage.color = normalColor;
            lockedImage.SetActive(false);
        }
    }

    public void SetEmptyLevelElement()
    {
        NameText.text = "More Soon";
        lockedImage.SetActive(false);
        button.interactable = false;
        NameText.color = LockedColor;
        buttonImage.color = LockedColor;
    }
}