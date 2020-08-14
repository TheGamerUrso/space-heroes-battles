using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class ObjectivesElement : MonoBehaviour
{

    [HideInInspector] public ObjectiveData objectiveData;

    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI CoinReward;
    [SerializeField] private TextMeshProUGUI ObjectiveDescriptionText = null;
    [SerializeField] private Button Button;
    [SerializeField] private Image ButtonImage;
    [SerializeField] private AudioClip ClickSoundEffect;
    [SerializeField] private GameObject CompletedGameObject = null;


    private bool completed;
    private float sleepTimer;
    private PlayerData playerData;
    private int currentPlayerLevel;


    private float xpToEarn;
    private int coinToEarn;


    public void InitializeObjective(ObjectiveData objectiveData)
    {
        Button.interactable = false;
        completed = objectiveData.completed;
        this.objectiveData = objectiveData;

        Button.onClick.AddListener(() =>
        {
            Complete();
        });

        RefreshQuests();

        playerData = PersistantData.GetPlayerData();
        currentPlayerLevel = playerData.GetCurrentPlayerShipData().level;

        switch ((ObjectiveTypeEnum)objectiveData.objectiveType)
        {
            case ObjectiveTypeEnum.KILL:
                xpToEarn = 50 * currentPlayerLevel;
                coinToEarn = 250;
                break;
            case ObjectiveTypeEnum.USE:
                xpToEarn = 20 * currentPlayerLevel;
                coinToEarn = 200;
                break;
            case ObjectiveTypeEnum.UNHARMED:
                xpToEarn = 75 * currentPlayerLevel;
                coinToEarn = 275;
                break;
            case ObjectiveTypeEnum.SURVIVE:
                xpToEarn = 25 * currentPlayerLevel;
                coinToEarn = 225;
                break;
            case ObjectiveTypeEnum.SPEND:
                xpToEarn = 15 * currentPlayerLevel;
                coinToEarn = objectiveData.progress / 3;
                break;
            case ObjectiveTypeEnum.BOUNTY:
                xpToEarn = 100 * currentPlayerLevel;
                coinToEarn = 300;
                break;
            case ObjectiveTypeEnum.SCORE:
                xpToEarn = 80 * currentPlayerLevel;
                coinToEarn = 280;
                break;
            default:
                break;
        }
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
            playerData.AddCoin(coinToEarn);
            playerData.EarnXP(xpToEarn);

            switch ((ObjectiveTypeEnum)objectiveData.objectiveType)
            {
                case ObjectiveTypeEnum.USE:
                    playerData.SetTotalSuperUsed(0);
                    break;
                case ObjectiveTypeEnum.UNHARMED:
                    playerData.SetHitInGame(false);
                    break;
                case ObjectiveTypeEnum.SURVIVE:
                    playerData.SetWaveSurvived(0);
                    break;
                case ObjectiveTypeEnum.SPEND:              
                    playerData.SetMoneySpend(0);
                    break;
            }

            completed = true;
            objectiveData.claimed = true;
            Button.interactable = false;
            RefreshQuests();

            Events.OnObjectiveChange?.Invoke(objectiveData);
        }
    }
    public void SetRewardInfo()
    {
        playerData = PersistantData.GetPlayerData();
        currentPlayerLevel = playerData.GetCurrentPlayerShipData().level;

        rewardText.text = xpToEarn + "xp";
        CoinReward.text = coinToEarn + "$";
    }

    public void RefreshQuests()
    {
        CompletedGameObject.SetActive(objectiveData.claimed);

        if ((ObjectiveTypeEnum)objectiveData.objectiveType == ObjectiveTypeEnum.UNHARMED || (ObjectiveTypeEnum)objectiveData.objectiveType == ObjectiveTypeEnum.BOUNTY)
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

        SetRewardInfo();
    }
}