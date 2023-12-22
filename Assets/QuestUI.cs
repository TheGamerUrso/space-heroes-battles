using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject[] ObjectiveLocations;

    private void OnEnable()
    {
        RefreshObjectives();
    }

    void Awake(){
        QuestSystem.Instance.LoadingNewQuests += LoadingNewObjectives;
        QuestSystem.Instance.OnNewQuestGenerated += InitializeObjectives;
        QuestSystem.Instance.OnQuestValueChanged+= RefreshObjectives;
    }

    private void Start()
    {
        InitializeObjectives();
        RefreshObjectives();
    }

    public void LoadingNewObjectives()
    {
        GameObject objectiveGO;
        for (int i = 0; i < ObjectiveLocations.Length; i++)
        {
            objectiveGO = ObjectiveLocations[i];
            objectiveGO.GetComponent<ObjectivesElement>().LoadingIndicator();
        }
    }

    public void RefreshObjectives()
    {
        GameObject objectiveGO;
        for (int i = 0; i < ObjectiveLocations.Length; i++)
        {
            objectiveGO = ObjectiveLocations[i];
            objectiveGO.GetComponent<ObjectivesElement>().RefreshQuests();
        }
    }

    public void InitializeObjectives()
    {
        var objectiveIndex = 0;
        GameObject objectiveGO;
        ObjectivesElement objectivesElement;
        foreach (ObjectiveData item in PersistantData.GetPlayerData().ListOfOnGoingObjectives.ToList())
        {
            objectiveGO = ObjectiveLocations[objectiveIndex];
            objectivesElement = objectiveGO.GetComponent<ObjectivesElement>();
            objectivesElement.InitializeObjective(item);
            objectiveIndex++;
        }
    }

    public void PlayButton()
    {
        // LevelEnum[] levels ={LevelEnum.Level0,LevelEnum.Level1,LevelEnum.Level2,LevelEnum.Level3,LevelEnum.Level4,LevelEnum.Level5,LevelEnum.Level6,LevelEnum.Level7,LevelEnum.Level8,LevelEnum.Level9};
        LevelEnum[] levels = { LevelEnum.Level0 };
        GameManager.Instance.LoadScene(levels[UnityEngine.Random.Range(0, levels.Length)]);
    }
}