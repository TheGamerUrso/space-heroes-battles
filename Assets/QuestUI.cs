using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject[] questLocation;

    private void OnEnable()
    {
        RefreshObjectives();
    }
    
    private void OnDestroy()
    {
        QuestSystem.Instance.LoadingNewQuests -= LoadingNewQuests;
        QuestSystem.Instance.OnNewQuestGenerated -= InitializeObjectives;
        QuestSystem.Instance.OnQuestValueChanged -= RefreshObjectives;
    }

    void Awake()
    {
        QuestSystem.Instance.LoadingNewQuests += LoadingNewQuests;
        QuestSystem.Instance.OnNewQuestGenerated += InitializeObjectives;
        QuestSystem.Instance.OnQuestValueChanged += RefreshObjectives;
    }

    private void Start()
    {
        InitializeObjectives();
    }

    public void LoadingNewQuests()
    {
        GameObject questGO;
        for (int i = 0; i < questLocation.Length; i++)
        {
            questGO = questLocation[i];
            questGO.GetComponent<QuestUIElement>().LoadingIndicator();
        }
    }

    public void RefreshObjectives()
    {
        GameObject questGO;
        for (int i = 0; i < QuestSystem.Instance.ListOfActiveQuest.Count; i++)
        {
            questGO = questLocation[i];
            questGO.GetComponent<QuestUIElement>().RefreshQuests();
        }
    }

    public void InitializeObjectives()
    {
        var questIndex = 0;
        foreach (QuestData item in PersistantData.GetPlayerData().ListOfPlayerActiveQuest.ToList())
        {
            var questLocation = this.questLocation[questIndex];
            var questUIElement = questLocation.GetComponent<QuestUIElement>();
            questUIElement.InitializeObjective(item);
            questIndex++;
        }
        RefreshObjectives();
    }

    public void PlayButton()
    {
        // LevelEnum[] levels ={LevelEnum.Level0,LevelEnum.Level1,LevelEnum.Level2,LevelEnum.Level3,LevelEnum.Level4,LevelEnum.Level5,LevelEnum.Level6,LevelEnum.Level7,LevelEnum.Level8,LevelEnum.Level9};
        LevelEnum[] levels = { LevelEnum.Level0 };
        GameManager.Instance.LoadScene(levels[UnityEngine.Random.Range(0, levels.Length)]);
    }
}