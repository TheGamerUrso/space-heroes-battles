using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Action<BaseEnemy> EnemyDied;

    public Action SpawnEnded;
    public Action<int, int, int> GameStatsChanged;
    public Action<string, BaseEnemy> BossDied;

    public List<EnemyElement> enemyElements;
    public Dictionary<string, EnemyElement> ListOfEnemyElements = new Dictionary<string, EnemyElement>();
    public bool HasBoss;

    [SerializeField] private int numberOfEnemiesEachWave;
    [Range(1, 16)]
    [SerializeField] private int waves;

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

    public List<EnemyElement> availableEnemie;
    public List<EnemyElement> tempList;
    public bool pause;

    public int EnemySpawnedInTotal { get; set; }

    private void Start()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetActiveScene().name.Equals("Gameplay"))
            {
                Debug.Log("Gameplay Scene active");
                continue;
            }
            Debug.Log("Not Gameplay Scene active");
            MissionCollection missionCollection = DataController.GetMissionCollection();
            Mission mission = missionCollection.GetMission(GameManager.LevelSelected);
            LevelDifficulty = mission.Level;

        }


        string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);

        TotalEnemies = numberOfEnemiesEachWave * waves;

        for (int i = 0; i < availableEnemies; i++)
        {
            ListOfEnemyElements.Add(enemyElements[i].Name, enemyElements[i]);
        }

        StartCoroutine(Spawn());
    }

    private void LateUpdate()
    {
        pause = false;
        if (Enemies.Count >= 8)
        {
            pause = true;
        }
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

                availableEnemie = enemyElements.GetRange(0, availableEnemies);
                tempList = availableEnemie.Where(x => (x.currentNumberInScene < x.MaxNumberInScene)).ToList();
                var range = 0;

                for (int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i].presentage > 0f)
                    {
                        range += tempList[i].presentage;
                    }
                }

                var rand = UnityEngine.Random.Range(0, range);
                var top = 0;

                for (int i = 0; i < tempList.Count; i++)
                {
                    top += tempList[i].presentage;
                    if (rand < top)
                    {
                        enemyElement = tempList[i];
                        randomNumb = i;
                        break;
                    }
                }

                while (pause)
                {
                    yield return new WaitForEndOfFrame();
                }

                // randomNumb = UnityEngine.Random.Range(0, tempList.Count); 

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

            if (HasBoss)
                GuiManager.PlayTrasmition(null, true);

            yield return new WaitForSeconds(cooldown);

            while (Enemies.Count > 0)
            {
                yield return new WaitForSeconds(cooldown);
            }

            if (HasBoss)
            {
                if (!BossBattleInitiated)
                {
                    BossBattleInitiated = true;
                    Debug.Log("Boss Battle");

                    SpawnBoss();
                }
            }
            else
            {
                BossBattleInitiated = true;
            }
        }

        Debug.Log("Game Over");

        if (!HasBoss)
        {
            SpawnEnded?.Invoke();
        }
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


        PlayerShip playerShip = PlayerManager.GetPlayer();
        int PlayerLevel = playerShip.Level;

        int EnemyLevel = baseEnemy.level;

        int levelDiffrence = PlayerLevel / EnemyLevel;

        if (levelDiffrence == 0)
        {
            levelDiffrence = 1;
        }

        float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;

        playerShip.AddXP(XPEarned);

        playerShip.IncreasePowerUp(.1f);

        //int dif = baseEnemy.level - playerShip.level;
        //if (dif > 0)
        //{
        //    float level = 25 / baseEnemy.level;
        //    playerShip.AddXP(baseEnemy.level);
        //}

        int score = GameSession.multiplier * baseEnemy.m_ValueOfEnemy;

        GameSession.Score += score;


        for (int i = 0; i < UnityEngine.Random.Range(4, 8); i++)
        {
            DropController.PickRandomDropItem(baseEnemy.transform);
        }

    }

    public void BossGotHit(string id, BaseEnemy baseEnemy)
    {
        PlayerShip playerShip = PlayerManager.GetPlayer();
        playerShip.PowerUpLevel += .1f;
    }

    public void EnemyEscapedCallback(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.enemyElement.currentNumberInScene--;
        baseEnemy.EnemyEscaped -= EnemyEscapedCallback;
        baseEnemy.EnemyDied -= EnemyDiedCallback;
        baseEnemy.EnemyGotHit -= EnemyGotHitCallback;
        Debug.Log("Enemy Got Escaped");
        Enemies.Remove(baseEnemy.gameObject);
        GameSession.enemyEscaped++;
    }

    public void EnemyGotHitCallback(string id, BaseEnemy baseEnemy)
    {
        Debug.Log("Enemy Got Hit");
        PlayerShip playerShip = PlayerManager.GetPlayer();
        playerShip.PowerUpLevel += .1f;
    }

    public void EnemyDiedCallback(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.enemyElement.currentNumberInScene--;
        baseEnemy.EnemyEscaped -= EnemyEscapedCallback;
        baseEnemy.EnemyDied -= EnemyDiedCallback;
        baseEnemy.EnemyGotHit -= EnemyGotHitCallback;

        GameSession.CurrentEnemyKilled++;
        GameSession.enemyKilled++;

        Debug.Log("Enemy Got Died");
        DropController.PickRandomDropItem(baseEnemy.transform);
        Enemies.Remove(baseEnemy.gameObject);

        PlayerShip playerShip = PlayerManager.GetPlayer();
        int PlayerLevel = playerShip.Level;

        int EnemyLevel = baseEnemy.level;

        int levelDiffrence = PlayerLevel / EnemyLevel;

        if (levelDiffrence == 0)
        {
            levelDiffrence = 1;
        }

        float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;

        playerShip.AddXP(XPEarned);

        playerShip.IncreasePowerUp(.1f);

        //int dif = baseEnemy.level - playerShip.level;
        //if (dif > 0)
        //{
        //    float level = 25 / baseEnemy.level;
        //    playerShip.AddXP(baseEnemy.level);
        //}

        int score = GameSession.multiplier * baseEnemy.m_ValueOfEnemy;

        GameSession.Score += score;

        EnemyDied?.Invoke(baseEnemy);


    }

}