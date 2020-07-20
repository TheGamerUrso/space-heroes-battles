using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectScreen : MonoBehaviour
{
    [SerializeField] private GameObject LevelElementContainer;
    [SerializeField] private GameObject LevelElementPrefab;
    [SerializeField] private LevelObjectivesElement[] levelObjectivesElement;

    private void OnEnable()
    {
        PersistantData.RefreshLevels();
    }

    private void Start()
    {
        InstansiateButtons();
    }

    public void InstansiateButtons()
    {
        List<Level> levels = PersistantData.GetLevels();
        var missionCollection = GameManager.Instance.GetMissions();

        var LevelElementGO = Instantiate(LevelElementPrefab, LevelElementContainer.transform, false);

        var levelElement = LevelElementGO.GetComponent<LevelElement>();

        levelElement.SetLevelElement(levels[0]);

        for (int i = 1; i < (levels.Count-1); i++)
        {
            var Level = levels[i];
            Mission missionItem = missionCollection.Missions[i];
            LevelElementGO = Instantiate(LevelElementPrefab, LevelElementContainer.transform, false);

            levelElement = LevelElementGO.GetComponent<LevelElement>();

            levelElement.SetLevelElement(Level);
        }

        GameObject emptyLevelElement = Instantiate(LevelElementPrefab, LevelElementContainer.transform, false);
        emptyLevelElement.GetComponent<LevelElement>().SetLevelElement(null);

        LevelDetailScreen.Instance.Setup();
    }


    public void Close()
    {
        ScreenManager.Instance.Close();
    }
}
