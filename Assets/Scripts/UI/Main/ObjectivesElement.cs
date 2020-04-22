using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveEventArgs : EventArgs
{
    public ObjectiveData objectiveData { get; set; }

    public ObjectiveEventArgs(ObjectiveData objectiveData)
    {
        this.objectiveData = objectiveData;
    }
}

public class ObjectivesElement : MonoBehaviour
{
    public event EventHandler<ObjectiveEventArgs> OnObjectiveChange;

    public static event Action<ObjectivesElement> OnClickEvent = delegate { };

    public bool completed;
    [HideInInspector] public ObjectiveData objectiveData;
    [SerializeField] private TextMeshProUGUI ObjectiveDescriptionText = null;
    public Button Button;
    public Image ButtonImage;
    [SerializeField] private AudioClip ClickSoundEffect;
    private float sleepTimer;
    [SerializeField] private GameObject CompletedGameObject = null;

    public void InitializeObjective(ObjectiveData objectiveData)
    {
        completed = objectiveData.completed;
        //Button.interactable = true;
        this.objectiveData = objectiveData;

        Button.onClick.AddListener(() =>
        {
            if (OnObjectiveChange != null) OnObjectiveChange(this, new ObjectiveEventArgs(objectiveData));
        });

        Button.interactable = false;

        RefreshQuests();
    }

    public void LoadingIndicator()
    {
        completed = false;
        ObjectiveDescriptionText.text = " Getting new Objective ";
        Button.interactable = false;
    }

    public void ResetStatus()
    {
        completed = false;
    }

    public void Complete()
    {

        if (!objectiveData.claimed && objectiveData.completed)
        {
            PlayerData playerData = PersistantData.GetPlayerData();

            switch ((ObjectiveType)objectiveData.objectiveType)
            {
                case ObjectiveType.Kill:
                    playerData.AddCoin(40);
                    playerData.EarnXP(10);
                    break;
                case ObjectiveType.Use:
                    playerData.AddCoin(40);
                    playerData.EarnXP(10);
                    playerData.SetTotalSuperUsed(0);
                    break;
                case ObjectiveType.Unharmed:
                    playerData.AddCoin(40);
                    playerData.EarnXP(10);
                    playerData.SetHitInGame(false);
                    break;
                case ObjectiveType.survive:
                    playerData.AddCoin(40);
                    playerData.EarnXP(10);
                    playerData.SetWaveSurvived(0);
                    break;
                case ObjectiveType.spend:
                    int moneySpend = objectiveData.progress;
                    playerData.AddCoin(moneySpend / 3);
                    playerData.EarnXP(10);
                    playerData.SetMoneySpend(0);
                    break;

                default:
                    break;
            }

            completed = true;
            objectiveData.claimed = true;
            Button.interactable = false;
            RefreshQuests();
        }  
    }

    public void RefreshQuests()
    {
        CompletedGameObject.SetActive(objectiveData.claimed);

        if ((ObjectiveType)objectiveData.objectiveType == ObjectiveType.Unharmed)
        {
            ObjectiveDescriptionText.text = objectiveData.Description;
            if (objectiveData.completed && !objectiveData.claimed)
            {
                Button.interactable = true;
            }
        }
        else
        {
            ObjectiveDescriptionText.text = objectiveData.Description.Replace("X", "" + objectiveData.progress);
            if (objectiveData.completed && !objectiveData.claimed)
            {
                Button.interactable = true;
            }

        }    
    }
}