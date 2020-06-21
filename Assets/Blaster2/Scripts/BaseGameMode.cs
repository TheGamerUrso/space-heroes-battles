using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseGameMode : MonoSingleton<BaseGameMode>
{
    protected PlayerShip playerShip;
    protected EnemyElement enemyElement;
    protected bool GameEnded;
    protected bool BossBattleInitiated;
    protected GameObject currentBoss;
    protected PlayerData playerData;
    [SerializeField] protected bool HasBoss;

    [SerializeField] protected int numberOfEnemiesEachWave;
    [Range(1, 16)]
    [SerializeField] protected int waves;
    [SerializeField] protected int availableEnemies;
    [SerializeField] protected int LevelDifficulty = 1;
    [SerializeField] protected float cooldown;
    [SerializeField] protected float delay = 0;
    [SerializeField] protected GameObject BossPrefab;
    protected bool pause;

    public int TotalEnemies;

    public Action<BaseEnemy> EnemyDied;
    public Action SpawnEnded;
    public Action<int, int, int> GameStatsChanged;
    public Action<string, BaseEnemy> BossDied;

    [SerializeField] protected List<EnemyElement> enemyElements;
    [SerializeField] protected Dictionary<string, EnemyElement> ListOfEnemyElements = new Dictionary<string, EnemyElement>();

    protected List<GameObject> Enemies = new List<GameObject>();
    protected List<EnemyElement> availableEnemie;
    protected List<EnemyElement> tempList;


    public int EnemySpawnedInTotal { get; set; }

    public void InitReference(PlayerData playerData, PlayerShip playerShip)
    {
        this.playerData = playerData;
        this.playerShip = playerShip;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    public void Start()
    {
        OnStart();
    }

    public virtual void OnStart()
    {
        MissionCollection missionCollection = PersistantData.GetMissionCollection();

        Scene scene = SceneManager.GetActiveScene();
        string index = scene.name[scene.name.Length - 1].ToString();
        Mission mission = missionCollection.GetMission(int.Parse(index));
        LevelDifficulty = mission.Level;

        TotalEnemies = numberOfEnemiesEachWave * waves;

        GameSession.EnemySpawnInTotal = TotalEnemies;

        for (int i = 0; i < availableEnemies; i++)
        {
            ListOfEnemyElements.Add(enemyElements[i].Name, enemyElements[i]);
        }

        StartCoroutine(StartGameDelay());
    }


    public void LateUpdate()
    {
        pause = false;
        if (Enemies.Count >= 8)
        {
            pause = true;
        }
    }

    protected IEnumerator StartGameDelay()
    {
        yield return new WaitForSeconds(2.0f);
        StartGame();
    }

    public virtual void StartGame()
    {
        string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);

        StartCoroutine(Spawn());
    }

    public virtual IEnumerator Spawn()
    {
        yield return null;
    }

    public void GameOver()
    {
        GameEnded = true;
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

        BossBattleInitiated = false;
        GameEnded = true;
    }

    public void BossGotHit(string id, BaseEnemy baseEnemy)
    {
        if (playerData != null)
            playerData.PowerUpLevel += .025f;
    }

    public void EnemyEscapedCallback(string id, BaseEnemy baseEnemy)
    {
        baseEnemy.enemyElement.currentNumberInScene--;
        Enemies.Remove(baseEnemy.gameObject);
        GameSession.enemyEscaped++;
    }

    public void EnemyGotHitCallback(string id, BaseEnemy baseEnemy)
    {
        if (playerData != null)
            playerData.PowerUpLevel += .025f;
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
