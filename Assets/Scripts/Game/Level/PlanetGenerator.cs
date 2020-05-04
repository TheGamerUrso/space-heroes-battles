using System.Collections;
using TheGamerUrso;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    public GameObject Planet;
    public GameObject[] Planets;
    private bool SpawnPlanets;
    void Start()
    {
        StartCoroutine(GeneratePlanet());
        SpawnPlanets = true;
    }

    IEnumerator GeneratePlanet()
    {
        while (SpawnPlanets)
        {
            yield return new WaitForSeconds(Random.Range(10, 20));
            GameObject planet = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.Planet) as GameObject;
            planet.transform.position = new Vector3(UnityEngine.Random.Range(-600, 600), -500, 2300);
            planet.transform.rotation = Quaternion.identity;
        }
    }
}
