using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseGameMode : MonoSingleton<BaseGameMode>
{
    public Level_SO level_SO;

    protected PlayerShip playerShip;
    protected EnemyElement enemyElement;
    protected bool BossBattleInitiated;
    protected GameObject currentBoss;
    protected PlayerData playerData;

    [Range(1, 16)]
    public int waves;
    [Range(1, 7)]
    public int availableEnemies;
    [Range(1, 20)]
    public int LevelDifficulty = 1;

    [SerializeField] protected float cooldown;
    [SerializeField] protected float delay = 0;

    protected bool pause;

    public int TotalEnemies;


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

    private void OnDestroy()
    {
        Events.EnemyDied -= EnemyDiedCallback;
        Events.EnemyEscaped -= EnemyEscapedCallback;
        Events.BossDied -= BossDiedCallback;
        Events.EnemyGotHit -= BossGotHit;
    }

    public virtual void Start()
    {
        Events.EnemyEscaped += EnemyEscapedCallback;
        Events.EnemyDied += EnemyDiedCallback;
        Events.BossDied += BossDiedCallback;
        Events.EnemyGotHit += EnemyGotHitCallback;
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

    public void BossEscapedCallback(string id, BaseEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            Enemies.Remove(baseEnemy.gameObject);
        }
    }

    public void BossGotHit(string id, BaseEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            playerData.SetPowerUpAmmount(0.025f);
        }
    }

    public void EnemyEscapedCallback(string id, BaseEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            baseEnemy.enemyElement.currentNumberInScene--;
            Enemies.Remove(baseEnemy.gameObject);
            Game.EnemyEscaped++;
        }
    }

    public void EnemyGotHitCallback(string id, BaseEnemy baseEnemy)
    {
        playerData.SetPowerUpAmmount(0.025f);
    }

    public virtual void BossDiedCallback(string id, BaseEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            baseEnemy.enemyElement.currentNumberInScene--;

            DropController.PickRandomDropItem(baseEnemy.transform);

            Enemies.Remove(baseEnemy.gameObject);

            TotalEnemies--;

            BossBattleInitiated = false;

            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.BOUNTY, int.Parse(id));
        }
    }

    public void EnemyDiedCallback(string id, BaseEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            baseEnemy.enemyElement.currentNumberInScene--;

            Enemies.Remove(baseEnemy.gameObject);

            DropController.PickRandomDropItem(baseEnemy.transform);
        }
    }

    
}
