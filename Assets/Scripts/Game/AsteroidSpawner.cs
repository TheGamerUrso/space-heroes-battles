using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public PoolGameObjectType[] AsteroidType;
    private Vector3 spawnPos;
    public Vector2 yMaxSpawn;

    private void Start()
    {
        StartCoroutine(SpawnerCoroutine());
    }

    private IEnumerator SpawnerCoroutine()
    {
        while (true)
        {
            System.Random rand = new System.Random();
            yield return new WaitForSeconds(rand.Next(1, 3));

            var spawnAsteroid = rand.Next(100);

            if (spawnAsteroid <= 90)
            {
                SpawnAsteroid();
            }
        }
    }

    public void SpawnAsteroid()
    {
        PoolGameObjectType poolGameObjectType = PoolGameObjectType.Asteroid1;

        int randomNumber = UnityEngine.Random.Range(0, AsteroidType.Length);

        poolGameObjectType = AsteroidType[randomNumber];


        GameObject asteroid = PoolManager.Instance.GetObjectFromPool(poolGameObjectType);

        float ypos = UnityEngine.Random.Range(yMaxSpawn.x, yMaxSpawn.y);
        spawnPos = new Vector3(UnityEngine.Random.Range(Constants.m_XMin, Constants.m_XMax), ypos, (Constants.m_ZMax) + (ypos * -1));

        asteroid.transform.position = spawnPos;
    }
}