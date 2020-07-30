using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct GameInfo
{
    public bool pause;
    public bool BossBattleInitiated;
    public int waves;
    public int LevelDifficulty;

    public void Reset()
    {
        pause = false;
        BossBattleInitiated = false;
    }
}

[Serializable]
public struct SpawnInfo
{
    public int TotalEnemies;
    public int availableEnemies;
    public List<EnemyElement> enemyElements;

    public void Reset()
    {
        enemyElements.Clear();
    }
}

public class BaseGameMode : MonoSingleton<BaseGameMode>
{
    public Level_SO level_SO;

    protected PlayerShip playerShip;
    protected EnemyElement enemyElement;

    protected GameObject currentBoss;
    protected PlayerData playerData;

    public GameInfo gameInfo;
    public SpawnInfo spawnInfo;

    protected float cooldown = 1f;
    protected float delay = 0.5f;

    protected List<GameObject> Enemies = new List<GameObject>();
    protected List<EnemyElement> availableEnemie = new List<EnemyElement>();
    protected List<EnemyElement> tempList = new List<EnemyElement>();
    protected Dictionary<string, EnemyElement> ListOfEnemyElements = new Dictionary<string, EnemyElement>();


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
        gameInfo.pause = false;
        if (Enemies.Count >= 8)
        {
            gameInfo.pause = true;
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
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
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
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
    }

    public virtual void BossDiedCallback(string id, BaseEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            baseEnemy.enemyElement.currentNumberInScene--;

            DropController.PickRandomDropItem(baseEnemy.transform);

            Enemies.Remove(baseEnemy.gameObject);

            spawnInfo.TotalEnemies--;

            gameInfo.BossBattleInitiated = false;

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
    public EnemyElement GetEnemyElemeny(string id)
    {
        EnemyElement enemyElement;
        if (ListOfEnemyElements.TryGetValue(id, out enemyElement))
        {
            return enemyElement;
        }
        return enemyElement;
    }

}
