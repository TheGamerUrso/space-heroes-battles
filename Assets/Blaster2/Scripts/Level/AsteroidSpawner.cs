using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private PoolGameObjectType[] AsteroidType;
    [SerializeField] private Transform[] spawnPos;
    private Coroutine AsteroidSpawnCoroutine;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(4);

    public void EnableAsteroids()
    {
         AsteroidSpawnCoroutine = StartCoroutine(SpawnerCoroutine());
    }

    public void DeactivateAsteroid(){
        if (AsteroidSpawnCoroutine != null)
        {
            AsteroidMove[] asteroids = GameObject.FindObjectsOfType<AsteroidMove>();
            for (int i = 0; i < asteroids.Length; i++)
            {
                asteroids[i].gameObject.SetActive(false);
            }
            StopCoroutine(AsteroidSpawnCoroutine);
        }
    }

    private IEnumerator SpawnerCoroutine()
    {
        while (true)
        {
            ChooseRandomWaitTimer();
            yield return waitForSeconds;
            SpawnAsteroid();
        }
    }

    public void SpawnAsteroid()
    {
        var poolGameObjectType = ChooseRandomAsteroids();
        GameObject asteroid = PoolManager.Instance.GetObjectFromPool(poolGameObjectType);
        asteroid.transform.position = ChooseRandomSpawnLocation();
        asteroid.SetActive(true);
    }

    public PoolGameObjectType ChooseRandomAsteroids() => AsteroidType[Random.Range(0, AsteroidType.Length)];

    public Vector3 ChooseRandomSpawnLocation() => new Vector3(Random.Range(-70f,71f),0,135);

    public void ChooseRandomWaitTimer() => waitForSeconds = new WaitForSeconds(Random.Range(4, 8));
}