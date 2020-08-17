using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private PoolGameObjectType[] AsteroidType;
    [SerializeField] private Transform[] spawnPos;
    private Coroutine AsteroidSpawnCoroutine;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(4);

    IEnumerator Start()
    {
        yield return waitForSeconds;
        AsteroidSpawnCoroutine = StartCoroutine(SpawnerCoroutine());
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

    public Vector3 ChooseRandomSpawnLocation() => spawnPos[Random.Range(0, spawnPos.Length)].position;

    public void ChooseRandomWaitTimer() => waitForSeconds = new WaitForSeconds(Random.Range(8, 16));
}