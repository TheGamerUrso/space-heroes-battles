using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EnemyElement
{
    public string Name;
    public PoolGameObjectType gameObjectType;
}

[Serializable]
public class GameInfo
{
    public bool pause;
    public bool BossBattleInitiated;
    public int waves;
    public int LevelDifficulty;

    public int TotalEnemies;
    public int CurrentTotalEnemies;
    public int availableEnemies;

    public List<EnemyElement> enemyElements;

    public GameInfo(List<EnemyElement> enemyElements)
    {
        pause = false;
        BossBattleInitiated = false;
        this.enemyElements = enemyElements;
    }
}

public class BaseGameMode : MonoBehaviour
{
    public int MaxNumberOfEnemies;
    public Level_SO level_SO;
    public GameInfo gameInfo;
    protected PlayerShip playerShip;
    protected EnemyElement enemyElement;
    [SerializeField] protected GameObject[] BossFights;
    protected GameObject currentBoss;
    protected GameObject spawnedBoss;
    protected PlayerData playerData;
    [SerializeField] protected float cooldown = 1f;
    protected float delay = 0.5f;
    protected List<GameObject> Enemies = new List<GameObject>();
    protected WaitForSeconds shortDelay;
    protected WaitForSeconds CooldownTimer;
    protected WaitForSeconds shortWait = new WaitForSeconds(1);
    protected WaitForSeconds longWait = new WaitForSeconds(2);
    protected WaitForSeconds RewardWait = new WaitForSeconds(5);
    [SerializeField] protected List<EnemySpawner> SpawnPoints = new List<EnemySpawner>();

    public int EnemySpawnedInTotal { get; set; }

    public virtual void SetGameMode()
    {
        playerShip = PlayerManager.GetPlayer();
        shortDelay = new WaitForSeconds(delay);
        CooldownTimer = new WaitForSeconds(cooldown);

        gameInfo.availableEnemies = level_SO.availableEnemies;
        gameInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;
        gameInfo.CurrentTotalEnemies = gameInfo.TotalEnemies;
        Game.EnemySpawnInTotal = gameInfo.TotalEnemies;
    }

    private void OnDestroy()
    {
        Events.EnemyDied -= EnemyDiedCallback;
        Events.EnemyEscaped -= EnemyEscapedCallback;
        Events.BossDied -= BossDiedCallback;
    }

    public virtual void Awake()
    {
        Events.EnemyEscaped += EnemyEscapedCallback;
        Events.EnemyDied += EnemyDiedCallback;
        Events.BossDied += BossDiedCallback;
    }

    public virtual void Start()
    {
        playerData = PersistantData.GetPlayerData();
        SetGameMode();
        StartCoroutine(StartGameDelay());
    }

    public void LateUpdate()
    {
        gameInfo.pause = false;
        if (Enemies.Count >= 8)
        {
            gameInfo.pause = true;
        }
    }

    protected virtual IEnumerator StartGameDelay()
    {
        yield return CooldownTimer;
        StartCoroutine(UpdateGameMode());
    }

    public virtual void Spawn() { }
    public virtual void SpawnBoss() { }
    public virtual IEnumerator UpdateGameMode()
    {
        yield return null;
    }

    public static BossEnemy SpawnBoss(GameObject BossPrefab, int LevelDifficulty = 1)
    {
        AudioManager.Instance.PlayMusicById("Boss", true);
        GameObject currentBoss = GameObject.Instantiate(BossPrefab);
        currentBoss.name = BossPrefab.name;

        BossEnemy enemy = currentBoss.GetComponentInChildren<BossEnemy>();
        enemy.Id = currentBoss.name;
        enemy.SetStats(LevelDifficulty);

        return enemy;
    }

    //Callbacks

    public virtual void EnemyDiedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(baseEnemy.Id))
        {
            Enemies.Remove(baseEnemy.gameObject);
        }
    }

    public virtual void BossDiedCallback(string id, BossEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            Enemies.Remove(baseEnemy.gameObject);
            gameInfo.BossBattleInitiated = false;
        }
    }
    public void EnemyEscapedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            Enemies.Remove(baseEnemy.gameObject);
        }
    }
    public void BossEscapedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            Enemies.Remove(baseEnemy.gameObject);
        }
    }


}
