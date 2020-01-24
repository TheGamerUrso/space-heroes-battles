using System.Collections;
using TheGamerUrso;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    public GameObject Planet;
    public GameObject[] Planets;

    void Start()
    {
        StartCoroutine(GeneratePlanet());
    }

    IEnumerator GeneratePlanet()
    {
        while (GameManager.IsGameOver == false)
        {
            yield return new WaitForSeconds(Random.Range(10, 20));
            GameObject planet = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.Planet) as GameObject;
            planet.transform.position = new Vector3(UnityEngine.Random.Range(-600, 600), -500, 2300);
            planet.transform.rotation = Quaternion.identity;
        }
    }
}
