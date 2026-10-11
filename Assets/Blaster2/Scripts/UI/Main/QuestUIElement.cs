using System;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class QuestUIElement : MonoBehaviour
{
    public event Action<int> OnButtonPressed;
    private int questIndex = 0;

    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI CoinReward;
    [SerializeField] private TextMeshProUGUI questDescription = null;
    [SerializeField] private Button Button;
    [SerializeField] private Image ButtonImage;
    [SerializeField] private AudioClip ClickSoundEffect;
    [SerializeField] private GameObject completedQuestGameObject = null;

    //=================================================================================
    public void SetElement(int questIndex,QuestData questData)
    {
        this.questIndex = questIndex;
        Button.interactable = false;

        completedQuestGameObject.SetActive(questData.claimed);
        questDescription.text = questData.Description;


        Button.interactable = questData.claimed ? false : questData.IsCompleted();

        rewardText.text = questData.questReward.XP + "xp";
        CoinReward.text = questData.questReward.Currency + "$";
    }
//=================================================================================
    public void LoadingIndicator()
    {
        questDescription.text = " Getting new Objective ";
        Button.interactable = false;
    }
//=================================================================================
    public void Complete()
    {
        OnButtonPressed?.Invoke(questIndex);
    }
}