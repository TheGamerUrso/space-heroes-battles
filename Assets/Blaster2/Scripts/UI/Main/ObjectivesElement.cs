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

        playerData = PersistantData.GetPlayerData();
        currentPlayerLevel = playerData.GetCurrentPlayerShipData().level;

        switch ((ObjectiveTypeEnum)objectiveData.objectiveType)
        {
            case ObjectiveTypeEnum.KILL:
                xpToEarn = 50 * currentPlayerLevel;
                coinToEarn = 100 * currentPlayerLevel;
                break;
            case ObjectiveTypeEnum.USE:
                xpToEarn = 20 * currentPlayerLevel;
                coinToEarn = 40 * currentPlayerLevel;
                break;
            case ObjectiveTypeEnum.UNHARMED:
                xpToEarn = 75 * currentPlayerLevel;
                coinToEarn = 150 * currentPlayerLevel;
                break;
            case ObjectiveTypeEnum.SURVIVE:
                xpToEarn = 25 * currentPlayerLevel;
                coinToEarn = 50 * currentPlayerLevel;
                break;
            case ObjectiveTypeEnum.SPEND:
                xpToEarn = 15 * currentPlayerLevel;
                coinToEarn = objectiveData.progress / 3 * currentPlayerLevel;
                break;
            case ObjectiveTypeEnum.BOUNTY:
                xpToEarn = 100 * currentPlayerLevel;
                coinToEarn = 200 * currentPlayerLevel;
                break;
            case ObjectiveTypeEnum.SCORE:
                xpToEarn = 80 * currentPlayerLevel;
                coinToEarn = 100 * currentPlayerLevel;
                break;
            default:
                break;
        }

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
            playerData.AddCoin(coinToEarn);
            playerData.EarnXP(xpToEarn);

            switch ((ObjectiveTypeEnum)objectiveData.objectiveType)
            {
                case ObjectiveTypeEnum.KILL:
                    playerData.SetPlayerKillsCounter(0);
                    break;
                case ObjectiveTypeEnum.USE:
                    playerData.SetUsedSuperCount(0);
                    break;
                case ObjectiveTypeEnum.UNHARMED:
                    playerData.SetPlayerGotHitCounter(false);
                    break;
                case ObjectiveTypeEnum.SURVIVE:
                    playerData.SetWaveSurvivedCount(0);
                    break;
                case ObjectiveTypeEnum.SPEND:
                    playerData.CoinSpend = 0;
                    break;
                case ObjectiveTypeEnum.BOUNTY:
                    playerData.BossBountyKilledId = -1;
                    break;
                case ObjectiveTypeEnum.SCORE: 
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

        if ((ObjectiveTypeEnum)objectiveData.objectiveType == ObjectiveTypeEnum.UNHARMED)
        {
            ObjectiveDescriptionText.text = objectiveData.Description;
        }
        else if ((ObjectiveTypeEnum)objectiveData.objectiveType == ObjectiveTypeEnum.BOUNTY || (ObjectiveTypeEnum)objectiveData.objectiveType == ObjectiveTypeEnum.SURVIVE)
        {
            ObjectiveDescriptionText.text = objectiveData.Description.Replace(" X ", "" + objectiveData.requirment);
        }
        else
        {
            ObjectiveDescriptionText.text = objectiveData.Description.Replace(" X ", "" + objectiveData.progress);
        }

        if (objectiveData.completed && !objectiveData.claimed)
        {
            Button.interactable = true;
        }

        SetRewardInfo();
    }
}