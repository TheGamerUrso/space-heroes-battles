using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


public class SpawnEnemies : Singleton<SpawnEnemies>
{
    public Action<int, int, int> GameStatsChanged;
    public Action<string, BaseEnemy> BossDied;

    public EnemyElement[] enemyElements;

    [SerializeField] private int numberOfEnemiesEachWave;

    [Range(0, 6)]
    public int availableEnemies;

    public int LevelDifficulty = 1;

    public int TotalEnemies;

    public float delay;


    private bool BossBattleInitiated;
    public GameObject BossPrefab;
    private GameObject currentBoss;



    public List<GameObject> Enemies = new List<GameObject>();

    public float cooldown;
    private EnemyElement enemyElement = null;

    public int EnemySpawnedInTotal { get; set; }
    private void Start()
    {
        MissionCollection missionCollection = DataController.GetMissionCollection();
        Mission mission = missionCollection.GetMission(GameManager.LevelSelected);
        LevelDifficulty = mission.Level;

        string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);

        StartCoroutine(Spawn());
    }

   

    IEnumerator Spawn()
    {
        Debug.Log("Game Started");
        while (!BossBattleInitiated)
        {
            while (GuiManager.IsTrasnmiting())
            {
                yield return new WaitForEndOfFrame();
            }

            while (TotalEnemies > 0)
            {
                int randomNumb = 0;


                List<EnemyElement> tempList = enemyElements.Where(x => (x.currentNumberInScene < x.MaxNumberInScene)).ToList();

                randomNumb = UnityEngine.Random.Range(0, availableEnemies);
                enemyElement = tempList[randomNumb];

                int repeat = 1;

                if (randomNumb == 0)
                {
                    repeat = UnityEngine.Random.Range(5, 8);
                }

                for (int i = 0; i < repeat; i++)
                {
                    if (TotalEnemies - 1 >= 0)
                    {
                        SpawnEnemyElement(enemyElement);
                    }
                    yield return new WaitForSeconds(delay);
                }

                yield return new WaitForSeconds(cooldown);
            }

            GuiManager.PlayTrasmition(null, true);

            yield return new WaitForSeconds(cooldown);

            while (Enemies.Count > 0)
            {
                yield return new WaitForSeconds(cooldown);
            }

            if (!BossBattleInitiated)
            {
                BossBattleInitiated = true;
                Debug.Log("Boss Battle");

                SpawnBoss();
            }

        }
        Debug.Log("Game Over");
    }

    private void SpawnBoss()
    {
        AudioManager.Instance.PlayMusicById("Boss", true);
        currentBoss = Instantiate(BossPrefab);
        BaseEnemy enemy = currentBoss.GetComponent<BaseEnemy>();

        enemy.SetEnemyStats(LevelDifficulty);

        enemy.EnemyEscaped = BossEscapedCallback;
        enemy.EnemyDied += BossDiedCallback;
        enemy.EnemyGotHit += BossGotHit;

        TotalEnemies++;
        Enemies.Add(currentBoss);
    }

    private void SpawnEnemyElement(EnemyElement enemyElement)
    {
        enemyElement.currentNumberInScene++;
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

        enemy.EnemyEscaped += EnemyEscapedCallback;
        enemy.EnemyDied += EnemyDiedCallback;
        enemy.EnemyGotHit += EnemyGotHitCallback;

        TotalEnemies--;
        Enemies.Add(enemGO);
    }

    public void BossEscapedCallback(string id, BaseEnemy baseEnemy)
    {
        Enemies.Remove(baseEnemy.gameObject);
        baseEnemy.EnemyEscaped -= EnemyEscapedCallback;
        baseEnemy.EnemyDied -= EnemyDiedCallback;
        baseEnemy.EnemyGotHit -= EnemyGotHitCallback;

    }

    public void BossDiedCallback(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.EnemyEscaped -= EnemyEscapedCallback;
        baseEnemy.EnemyDied -= EnemyDiedCallback;
        baseEnemy.EnemyGotHit -= EnemyGotHitCallback;
        BossDied?.Invoke(id, baseEnemy);
        Enemies.Remove(baseEnemy.gameObject);
        TotalEnemies--;
    }

    public void BossGotHit(string id, BaseEnemy baseEnemy)
    {

    }

    public void EnemyEscapedCallback(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.enemyElement.currentNumberInScene--;
        baseEnemy.EnemyEscaped -= EnemyEscapedCallback;
        baseEnemy.EnemyDied -= EnemyDiedCallback;
        baseEnemy.EnemyGotHit -= EnemyGotHitCallback;
        Debug.Log("Enemy Got Escaped");
        Enemies.Remove(baseEnemy.gameObject);
    }

    public void EnemyGotHitCallback(string id, BaseEnemy baseEnemy)
    {
        Debug.Log("Enemy Got Hit");
    }

    public void EnemyDiedCallback(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.enemyElement.currentNumberInScene--;
        baseEnemy.EnemyEscaped -= EnemyEscapedCallback;
        baseEnemy.EnemyDied -= EnemyDiedCallback;
        baseEnemy.EnemyGotHit -= EnemyGotHitCallback;

        Debug.Log("Enemy Got Died");

        Enemies.Remove(baseEnemy.gameObject);
    }

}