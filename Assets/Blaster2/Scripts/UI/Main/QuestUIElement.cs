using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class QuestUIElement : MonoBehaviour
{

     public QuestData questData;

    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI CoinReward;
    [SerializeField] private TextMeshProUGUI questDescription = null;
    [SerializeField] private Button Button;
    [SerializeField] private Image ButtonImage;
    [SerializeField] private AudioClip ClickSoundEffect;
    [SerializeField] private GameObject completedQuestGameObject = null;


    private bool completed;
    private float sleepTimer;
    private PlayerData playerData;
    private int currentPlayerLevel;


    private float xpToEarn;
    private int coinToEarn;


    public void InitializeObjective(QuestData objectiveData)
    {
        Button.interactable = false;
        completed = objectiveData.completed;
        this.questData = objectiveData;

        Button.onClick.AddListener(() =>
        {
            Complete();
        });

        playerData = PersistantData.GetPlayerData();
        currentPlayerLevel = playerData.GetCurrentPlayerShipData().level;

        switch ((QuestTypeEnum)objectiveData.questType)
        {
            case QuestTypeEnum.KILL:
                xpToEarn = 50 * currentPlayerLevel;
                coinToEarn = 100 * currentPlayerLevel;
                break;
            case QuestTypeEnum.USE:
                xpToEarn = 20 * currentPlayerLevel;
                coinToEarn = 40 * currentPlayerLevel;
                break;
            case QuestTypeEnum.UNHARMED:
                xpToEarn = 75 * currentPlayerLevel;
                coinToEarn = 150 * currentPlayerLevel;
                break;
            case QuestTypeEnum.SURVIVE:
                xpToEarn = 25 * currentPlayerLevel;
                coinToEarn = 50 * currentPlayerLevel;
                break;
            case QuestTypeEnum.SPEND:
                xpToEarn = 15 * currentPlayerLevel;
                coinToEarn = objectiveData.progress / 3 * currentPlayerLevel;
                break;
            case QuestTypeEnum.BOUNTY:
                xpToEarn = 100 * currentPlayerLevel;
                coinToEarn = 200 * currentPlayerLevel;
                break;
            case QuestTypeEnum.SCORE:
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
        questDescription.text = " Getting new Objective ";
        Button.interactable = false;
    }

    public void ResetStatus()
    {
        completed = false;
    }

    public void Complete()
    {
        if (!questData.claimed && questData.completed)
        {
            playerData.AddCoin(coinToEarn);
            playerData.EarnXP(xpToEarn);

            switch ((QuestTypeEnum)questData.questType)
            {
                case QuestTypeEnum.KILL:
                    playerData.SetPlayerKillsCounter(0);
                    break;
                case QuestTypeEnum.USE:
                    playerData.SetUsedSuperCount(0);
                    break;
                case QuestTypeEnum.UNHARMED:
                    playerData.SetPlayerGotHitCounter(false);
                    break;
                case QuestTypeEnum.SURVIVE:
                    playerData.SetWaveSurvivedCount(0);
                    break;
                case QuestTypeEnum.SPEND:
                    playerData.CoinSpend = 0;
                    break;
                case QuestTypeEnum.BOUNTY:
                    playerData.BossBountyKilledId = 0;
                    break;
                case QuestTypeEnum.SCORE: 
                    break;
            }

            questData.claimed = true;
            Button.interactable = false;
 
            QuestSystem.Instance.CompleteQuest(questData);
            RefreshQuests();
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
        completedQuestGameObject.SetActive(questData.claimed);

        if ((QuestTypeEnum)questData.questType == QuestTypeEnum.UNHARMED)
        {
            questDescription.text = questData.Description;
        }
        else if ((QuestTypeEnum)questData.questType == QuestTypeEnum.BOUNTY || (QuestTypeEnum)questData.questType == QuestTypeEnum.SURVIVE)
        {
            questDescription.text = questData.Description.Replace(" X ", "" + questData.requirment);
        }
        else
        {
            questDescription.text = questData.Description.Replace(" X ", "" + questData.progress);
        }

        if (questData.completed && !questData.claimed)
        {
            Button.interactable = true;
        }

        SetRewardInfo();
    }
}