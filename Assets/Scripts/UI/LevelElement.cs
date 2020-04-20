using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct Level
{
    public string ID;
    public Mission mission;
    public Sprite sprite;
    public bool interactable;
    public bool Locked;

    public Level(string ID, Mission mission, Sprite sprite, bool interactable, bool Locked)
    {
        this.ID = ID;
        this.mission = mission;
        this.sprite = sprite;
        this.interactable = interactable;
        this.Locked = Locked;
    }
}

public class LevelElement : MonoBehaviour
{
    private Level level;
    public Sprite[] sprites;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI NameText;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color LockedColor;
    public void SetLevelElement(Level level, Action<Level> buttonAction)
    {
        this.level = level;
        if (level.ID.Contains("Mission") || level.ID.Contains("Prologue"))
        {
            NameText.text = level.mission.Title + "\n" + "Level " + level.mission.Level + " Recomented";

            button.onClick.AddListener(() =>
            {
                Debug.Log("Pressed" + "Level " + level.mission.Level + " Button");
                buttonAction.Invoke(level);
            });
        }
        else if (level.ID.Contains("Survival"))
        {     
            NameText.text = "SurvivalMode";

            button.onClick.AddListener(() =>
            {
                Debug.Log("Pressed" + "Survival Mode");
                buttonAction.Invoke(level);
            });

            NameText.color = normalColor;
            buttonImage.color = normalColor;
        }
        else if (string.IsNullOrEmpty(level.ID))
        {
            SetEmptyLevelElement();
        }

        Refresh();
    }

    public void SetEmptyLevelElement()
    {
        NameText.text = "More Soon";
        level.interactable = false;
        level.Locked = true;
        button.interactable = level.interactable;
        NameText.color = LockedColor;
        buttonImage.color = LockedColor;
    }
    public void Lock()
    {
        level.interactable = false;
        button.interactable = level.interactable;
        
    }
    public void Unlock()
    {
        level.interactable = true;
        button.interactable = level.interactable;
       
    }

    public void Refresh()
    {
        if (level.ID.Contains("Mission") || level.ID.Contains("Prologue"))
        {
            PlayerData playerData = PersistantData.GetPlayerData();
            if (level.mission != null)
            {
                if (level.mission.ID <= playerData.LevelUnlocked)
                {
                    level.interactable = true;
                    level.Locked = false;
                }
            }

            button.interactable = level.interactable;

            if (level.Locked)
            {
                NameText.color = LockedColor;
                buttonImage.color = LockedColor;
            }
            else
            {
                NameText.color = normalColor;
                buttonImage.color = normalColor;
            }
        }
        else if (level.ID.Contains("Survival"))
        {
            level.interactable = true;
            level.Locked = false;
            NameText.color = normalColor;
            buttonImage.color = normalColor;
        }
        else if (string.IsNullOrEmpty(level.ID))
        {
            level.interactable = false;
            level.Locked = true;
            NameText.color = LockedColor;
            buttonImage.color = LockedColor;
        }
    }





}