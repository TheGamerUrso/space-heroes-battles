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

    protected BaseBossEnemy currentBoss;
    protected PlayerData playerData;

    public GameInfo gameInfo;
    public SpawnInfo spawnInfo;

    protected float cooldown = 1f;
    protected float delay = 0.5f;

    protected List<GameObject> Enemies = new List<GameObject>();
    protected List<EnemyElement> availableEnemie = new List<EnemyElement>();
    protected List<EnemyElement> tempList = new List<EnemyElement>();
    protected Dictionary<string, EnemyElement> ListOfEnemyElements = new Dictionary<string, EnemyElement>();

    protected WaitForSeconds waitForCooldown = new WaitForSeconds(1);
    protected WaitForSeconds waitForFourSeconds = new WaitForSeconds(4);
    protected WaitForSeconds waitForSec;

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

    protected virtual IEnumerator StartGameDelay()
    {
        yield return null;
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
            Enemies.Remove(baseEnemy.gameObject);

            spawnInfo.TotalEnemies--;

            gameInfo.BossBattleInitiated = false;

            int BossId = id[id.Length - 1];

            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.BOUNTY, BossId);
        }
    }

    public virtual void EnemyDiedCallback(string id, BaseEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(baseEnemy.Id))
        {
            baseEnemy.enemyElement.currentNumberInScene--;
            DropController.PickRandomDropItem(baseEnemy.transform);

            Enemies.Remove(baseEnemy.gameObject);

            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);

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
