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


public class BaseGameMode : MonoSingleton<BaseGameMode>
{
    public int MaxNumberOfEnemies;

    public Level_SO level_SO;

    protected PlayerShip playerShip;
    protected EnemyElement enemyElement;

    protected BossEnemy currentBoss;
    protected PlayerData playerData;

    public GameInfo gameInfo;

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

    public virtual void InitReference(PlayerData playerData, PlayerShip playerShip)
    {
        this.playerData = playerData;
        this.playerShip = playerShip;

    }

    private void OnDestroy()
    {
        Events.EnemyDied -= EnemyDiedCallback;
        Events.EnemyEscaped -= EnemyEscapedCallback;
        Events.BossDied -= BossDiedCallback;
        Events.EnemyGotHit -= EnemyGotHitCallback;
    }

    public virtual void Start()
    {
        Events.EnemyEscaped += EnemyEscapedCallback;
        Events.EnemyDied += EnemyDiedCallback;
        Events.BossDied += BossDiedCallback;
        Events.EnemyGotHit += EnemyGotHitCallback;
        Events.BossHit += BossEnemyHitCallback;
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
        yield return null;
    }

    public virtual IEnumerator Spawn()
    {
        yield return null;
    }

    public void BossEscapedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            Enemies.Remove(baseEnemy.gameObject);
        }
    }

    public void BossGotHit(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
        }
    }

    public void EnemyEscapedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            Enemies.Remove(baseEnemy.gameObject);
            Game.EnemyEscaped++;
        }
    }

    public void EnemyGotHitCallback(string id, Enemy baseEnemy)
    {
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.01f);
    }

    public void BossEnemyHitCallback(string id, BossEnemy bossEnemy)
    {
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.01f);
    }

    public virtual void BossDiedCallback(string id, BossEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            Enemies.Remove(baseEnemy.gameObject);

            gameInfo.BossBattleInitiated = false;

            int BossId = id[id.Length - 1];

            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.BOUNTY, BossId);
        }
    }

    public virtual void EnemyDiedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(baseEnemy.Id))
        {
            Enemies.Remove(baseEnemy.gameObject);

            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);

            if (!Game.IsSurvivalMode)
            {
                int PlayerLevel = playerData.GetCurrentPlayerShipData().level;
                int EnemyLevel = baseEnemy.Level;
                int levelDiffrence = PlayerLevel / EnemyLevel;

                if (levelDiffrence == 0)
                {
                    levelDiffrence = 1;
                }

                float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
                playerData.EarnXP(XPEarned);

                GuiManager.CreateFloatingText("<color=" + "yellow" + ">" + XPEarned + "</color>" + "<color=" + "orange" + "> XP </color>", baseEnemy.transform.localPosition);
            }

            int kills = Game.EnemyKilled + 1;
            int score = baseEnemy.EnemyData.EnemyValue;

            if (Game.Multiplier > 0)
            {
                score = Game.Multiplier * baseEnemy.EnemyData.EnemyValue;
                GuiManager.SetScoreMultipler(score + "(x" + Game.Multiplier + ")");
            }
            else
            {
                GuiManager.SetScoreMultipler("" + score);
            }

            Game.SetCurrentEnemyKills(kills);

            Game.SetScore(score);

            Game.IncreaseMultiplier();

            playerData.Kills = kills;

            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.KILL, 1);
            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.SCORE, score);
        }
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
}
