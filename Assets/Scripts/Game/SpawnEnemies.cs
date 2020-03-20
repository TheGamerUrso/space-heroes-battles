using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;

[Serializable]
public class EnemyElement
{
    public string Name;
    public PoolGameObjectType gameObjectType;
    public GameObject Prefab;
    public int currentNumberInScene;
    public int MaxNumberInScene;
    public int presentage;
}


public class SpawnEnemies : Singleton<SpawnEnemies>, IEndGameObserver
{
    public Action<GameObject> OnSpawnEnemy;

    public EnemyElement[] enemyElements;
    [Range(0, 6)]
    public int availableEnemies;

    public int NumberOfEnemies = 0;
    public int LevelDifficulty = 1;

    private int wave;
    private int maxWave;

    private float spawnTimer;
    public float delay;
    private bool spawnReady;

    public bool gameover;
    private int totalEnemies;

 

    private void Start()
    {
        spawnTimer = delay;


        string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);

        StartCoroutine(Spawn());


        GameController.Instance.GameStatsChanged += GameStatsChanged;
        GameController.Instance.AddObserver(this);
    }

    public void GameStatsChanged(int wave, int maxWave, int totalEnemies)
    {
        this.wave = wave;
        this.maxWave = maxWave;
        this.totalEnemies = totalEnemies;
    }

    public void NewWave()
    {
        NumberOfEnemies = 0;
        GameController.Instance.NewWave();
    }

    IEnumerator Spawn()
    {
        while (!gameover)
        {
            if (NumberOfEnemies % GameController.Instance.NumberOfEnemiesEachWave == 0)
            {
                NewWave();
            }

            while (GuiManager.IsTrasnmiting())
            {
                bool isTransmiting = GuiManager.IsTrasnmiting();
                yield return new WaitForEndOfFrame();
            }


            int randomNumb = UnityEngine.Random.Range(0, availableEnemies);
            EnemyElement enemyElement = enemyElements[randomNumb];


            int repeat = enemyElement.MaxNumberInScene;
            if (randomNumb == 0)
            {
                repeat = UnityEngine.Random.Range(5, 8);
            }

            for (int i = 0; i < repeat; i++)
            {
                Debug.Log(totalEnemies + " " + (totalEnemies - 1));
                if (totalEnemies - 1 >= 0)
                {
                    SpawnEnemyElement(enemyElement);
                    yield return new WaitForSeconds(UnityEngine.Random.Range(2, 3));
                }
                else
                {
                    //Boss or Gameover
                    break;
                }

            }

            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnEnemyElement(EnemyElement enemyElement)
    {

        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(Constants.m_XMin, Constants.m_XMax), -50, Constants.m_ZMax);
        GameObject enemGO = PoolManager.Instance.GetObjectFromPool(enemyElement.gameObjectType);

        BaseEnemy enemy = enemGO.GetComponent<BaseEnemy>();
        FollowPathAI followPathAI = enemGO.GetComponent<FollowPathAI>();

        if (followPathAI == null)
        {
            enemGO.transform.position = spawnPos;
            enemGO.transform.rotation = Quaternion.LookRotation(Vector3.back);
        }
        else
        {
            enemGO.transform.position = spawnPos;
            followPathAI.GeneratePath();
        }

        enemy.enemyElement = enemyElement;

        enemy.SetEnemyStats(LevelDifficulty);

        enemyElement.currentNumberInScene++;

        enemy.EnemyEscaped += OnEnemyEscape;
        enemy.EnemyDied += OnDeath;
        enemy.EnemyGotHit += OnHit;

        OnSpawnEnemy?.Invoke(enemy.gameObject);
    }

    public void OnEnemyEscape(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.EnemyEscaped -= OnEnemyEscape;
        baseEnemy.EnemyDied -= OnDeath;
        baseEnemy.EnemyGotHit -= OnHit;

        Debug.Log("Enemy Got Escaped");
    }

    public void OnHit(string id, BaseEnemy baseEnemy)
    {
        Debug.Log("Enemy Got Hit");
    }

    public void OnDeath(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.EnemyEscaped -= OnEnemyEscape;
        baseEnemy.EnemyDied -= OnDeath;
        baseEnemy.EnemyGotHit -= OnHit;

        Debug.Log("Enemy Got Died");
    }

    public void Notify()
    {
        Debug.Log("Game Over");
        StopAllCoroutines();
        gameover = true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameController.Instance.RemoveObserver(this);
    }


}