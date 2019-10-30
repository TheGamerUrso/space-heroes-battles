using UnityEngine;

public class LevelEventManager : MonoBehaviour
{
    public GameObject[] TriggerEventSpawnLocations;
    public GameObject TriggerEventPrefab;
    public GameObject EventHolder;

    private void Start()
    {
        foreach (GameObject item in TriggerEventSpawnLocations)
        {
            item.SetActive(false);
        }
    }

    public void ResetEvents()
    {
        foreach (GameObject item in TriggerEventSpawnLocations)
        {
            item.GetComponentInChildren<TriggerEvent>().ResetEvent();
        }

    }


    public void SpawnEvent()
    {
        foreach (GameObject item in TriggerEventSpawnLocations)
        {
            item.SetActive(false);
        }

        int randomNumber = Random.Range(0, 100);
        if (randomNumber < 25f)
        {
            GameObject BuildingEvent = TriggerEventSpawnLocations[Random.Range(0, TriggerEventSpawnLocations.Length)];
            BuildingEvent.SetActive(true);

        }
    }


}
