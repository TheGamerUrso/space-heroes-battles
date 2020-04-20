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

   protected PlayerShip playerShip;

    public Action SpawnEnded;
    public Action<int, int, int> GameStatsChanged;
    public Action<string, BaseEnemy> BossDied;

    public List<EnemyElement> enemyElements;
    public Dictionary<string, EnemyElement> ListOfEnemyElements = new Dictionary<string, EnemyElement>();
    public bool HasBoss;

    [SerializeField] protected int numberOfEnemiesEachWave;
    [Range(1, 16)]
    [SerializeField] protected int waves;

    [Range(0, 6)]
    public int availableEnemies;

    public int LevelDifficulty = 1;

    public int TotalEnemies;

    public float delay = 0;


    public GameObject BossPrefab;

    protected bool GameEnded;
    protected bool BossBattleInitiated;
    protected GameObject currentBoss;
    protected PlayerData playerData;

    public List<GameObject> Enemies = new List<GameObject>();

    public float cooldown;
    protected EnemyElement enemyElement = null;

    public List<EnemyElement> availableEnemie;
    public List<EnemyElement> tempList;
    public bool pause;

    public int EnemySpawnedInTotal { get; set; }


    public void InitReference(PlayerData playerData, PlayerShip playerShip)
    {
        this.playerData = playerData;
        this.playerShip = playerShip;
    }

    public void Start()
    {
        OnStart();
    }

    public virtual void OnStart()
    {
        playerShip = PlayerManager.GetPlayer();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetActiveScene().name.Equals("Gameplay"))
            {
                Debug.Log("Gameplay Scene active");
                continue;
            }
            Debug.Log("Not Gameplay Scene active");
            MissionCollection missionCollection = PersistantData.GetMissionCollection();
            Mission mission = missionCollection.GetMission(GameManager.LevelIndexSelected);
            LevelDifficulty = mission.Level;

        }


        string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);

        TotalEnemies = numberOfEnemiesEachWave * waves;

        GameSession.EnemySpawnInTotal = TotalEnemies;

        for (int i = 0; i < availableEnemies; i++)
        {
            ListOfEnemyElements.Add(enemyElements[i].Name, enemyElements[i]);
        }

        StartCoroutine(Spawn());
    }

    public void LateUpdate()
    {
        pause = false;
        if (Enemies.Count >= 8)
        {
            pause = true;
        }
    }

    IEnumerator  Spawn()
    {
        Debug.Log("Game Started");
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        WaitForSeconds waitForSec = new WaitForSeconds(delay);
        WaitForSeconds waitForCooldown = new WaitForSeconds(cooldown);
        WaitForSeconds waitforOneSec = new WaitForSeconds(1);
        WaitForSeconds waitForFourSeconds = new WaitForSeconds(4);

        while (!GameEnded)
        {
            while (GuiManager.Instance.IsTrasnmiting() || GameEnded)
            {
                yield return waitForEndOfFrame;
            }

            while (TotalEnemies > 0 && !GameEnded)
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
                    yield return waitForEndOfFrame;
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
                    yield return waitForSec;
                }

                yield return waitForCooldown;


            }

            if (HasBoss)
                GuiManager.PlayTrasmition(null, true);

            yield return waitForCooldown;

            while (Enemies.Count > 0)
            {
                yield return waitForCooldown;
            }

            if (HasBoss)
            {
                if (!BossBattleInitiated)
                {
                    BossBattleInitiated = true;
                    Debug.Log("Boss Battle");

                    SpawnBoss();
                }


                while (BossBattleInitiated)
                {
                    yield return waitforOneSec;
                }
            }
            else
            {
                GameOver();
            }
        }

        yield return waitForFourSeconds;

        if (!GameSession.IsGameOver)
        {
            SpawnEnded?.Invoke();
        }
    }

    public void GameOver()
    {
        GameEnded = true;
    }

    protected void SpawnBoss()
    {
        AudioManager.Instance.PlayMusicById("Boss", true);
        currentBoss = Instantiate(BossPrefab);
        BaseEnemy enemy = currentBoss.GetComponent<BaseEnemy>();

        enemy.SetEnemyStats(LevelDifficulty);

        TotalEnemies++;
        Enemies.Add(currentBoss);
    }

    protected void SpawnEnemyElement(EnemyElement enemyElement)
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

        TotalEnemies--;
        Enemies.Add(enemGO);
    }

    public void BossEscapedCallback(string id, BaseEnemy baseEnemy)
    {
        Enemies.Remove(baseEnemy.gameObject);
    }

    public virtual void BossDiedCallback(string id, BaseEnemy baseEnemy)
    {
        Enemies.Remove(baseEnemy.gameObject);

        TotalEnemies--;

        EnemyDied?.Invoke(baseEnemy);

        int rand = UnityEngine.Random.Range(4, 8);

        for (int i = 0; i < rand; i++)
        {
            DropController.PickRandomDropItem(baseEnemy.transform);
        }

        BossBattleInitiated = false;
        GameEnded = true;
    }

    public void BossGotHit(string id, BaseEnemy baseEnemy)
    {
        playerData.PowerUpLevel += .1f;
    }

    public void EnemyEscapedCallback(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.enemyElement.currentNumberInScene--;
        Enemies.Remove(baseEnemy.gameObject);
        GameSession.enemyEscaped++;
    }

    public void EnemyGotHitCallback(string id, BaseEnemy baseEnemy)
    {
        playerData.PowerUpLevel += .1f;
    }

    public void EnemyDiedCallback(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.enemyElement.currentNumberInScene--;

        Enemies.Remove(baseEnemy.gameObject);

        DropController.PickRandomDropItem(baseEnemy.transform);

        EnemyDied?.Invoke(baseEnemy);
    }

    public void UnregisterEnemy(BaseEnemy baseEnemy)
    {
        if (baseEnemy.GetComponent<BaseBossEnemy>() != null)
        {
            baseEnemy.EnemyEscaped -= EnemyEscapedCallback;
            baseEnemy.EnemyDied -= BossDiedCallback;
            baseEnemy.EnemyGotHit -= BossGotHit;
        }
        else
        {
            baseEnemy.EnemyEscaped -= EnemyEscapedCallback;
            baseEnemy.EnemyDied -= EnemyDiedCallback;
            baseEnemy.EnemyGotHit -= EnemyGotHitCallback;
        }
    }

    public void RegisterEnemy(BaseEnemy baseEnemy)
    {
        if (baseEnemy.GetComponent<BaseBossEnemy>() != null)
        {
            baseEnemy.EnemyEscaped += EnemyEscapedCallback;
            baseEnemy.EnemyDied += BossDiedCallback;
            baseEnemy.EnemyGotHit += BossGotHit;
        }
        else
        {
            baseEnemy.EnemyEscaped += EnemyEscapedCallback;
            baseEnemy.EnemyDied += EnemyDiedCallback;
            baseEnemy.EnemyGotHit += EnemyGotHitCallback;
        }
    }

}