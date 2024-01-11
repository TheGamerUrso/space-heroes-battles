using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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
    public Level_SO level_SO;
    public GameInfo gameInfo;
    [SerializeField] protected GameObject[] BossFights;
    [SerializeField] protected List<EnemySpawner> SpawnPoints = new List<EnemySpawner>();
    protected GameObject currentBoss;
    [SerializeField] protected float cooldown = 1f;
    protected float delay = 0.5f;
    protected WaitForSeconds shortDelay;
    protected WaitForSeconds CooldownTimer;
    protected WaitForSeconds shortWait = new WaitForSeconds(1);
    protected WaitForSeconds longWait = new WaitForSeconds(2);
    protected WaitForSeconds RewardWait = new WaitForSeconds(5);

    public virtual void SetGameMode()
    {
        shortDelay = new WaitForSeconds(delay);
        CooldownTimer = new WaitForSeconds(cooldown);

        gameInfo.availableEnemies = level_SO.availableEnemies;
        gameInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;
        gameInfo.CurrentTotalEnemies = gameInfo.TotalEnemies;
        GameController.Instance.EnemySpawnInTotal = gameInfo.TotalEnemies;
    }

    private void OnDestroy()
    {
        Events.EnemyDied -= OnEnemyDiedHandled;
        Events.BossDied -= OnEnemyDiedHandled;
        Events.EnemyGotHit -= OnEnemyHitHandled;
        Events.BossHit -= OnEnemyHitHandled;
        Events.EnemyEscaped -= OnEnemyEscapedCallback;
    }

    public virtual void Awake()
    {

        Events.EnemyDied += OnEnemyDiedHandled;
        Events.BossDied += OnEnemyDiedHandled;
        Events.EnemyGotHit += OnEnemyHitHandled;
        Events.BossHit += OnEnemyHitHandled;
        Events.EnemyEscaped += OnEnemyEscapedCallback;
    }

    public virtual void Start()
    {
        SetGameMode();
        StartCoroutine(StartGameDelay());
    }

    public void LateUpdate()
    {
        gameInfo.pause = false;
        if (Enemy.EnemiesCount >= 8)
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

    public void GameOver()
    {
        StartCoroutine(DelayGameOver());
    }
    IEnumerator DelayGameOver()
    {
        Time.timeScale = 1.0f;
        var playerData = PersistantData.GetPlayerData();
        playerData.GetCurrentPlayerShipData().Upgrades[(int)UpgradeTypeEnum.Shield] = 0;
        playerData.SetScore(GameController.Instance.Score);
        SaveSystem.SaveGame();

        AudioManager.PlayMusic("GameOver", false);

        yield return new WaitForSeconds(2.0f);

        Events.OnGameOver?.Invoke(this,false);
    }

    public void Win()
    {
        StartCoroutine(DelayWinScreen());
    }

    IEnumerator DelayWinScreen()
    {
        Time.timeScale = 1.0f;

        PersistantData.GetPlayerData().GetCurrentPlayerShipData().Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        AudioManager.PlayMusic("Victory", false);

        PlayerManager.GetPlayer()?.ExitLevel();

        yield return new WaitForSeconds(2.0f);
        Events.OnGameOver?.Invoke(this,true);
    }

    //=================================================================================
    public virtual void OnEnemyDiedHandled(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(baseEnemy.Id))
        {
            if (baseEnemy.GetComponent<BossEnemy>())
            {
                Debug.Log("You Killed A Boss");
            }
            else
            {
                Debug.Log("You Killed an Enemy");
            }

            var playerData = PersistantData.GetPlayerData();
            int PlayerLevel = playerData.GetCurrentPlayerShipData().level;
            int EnemyLevel = baseEnemy.Level;
            int levelDiffrence = PlayerLevel / EnemyLevel;
            if (levelDiffrence == 0) levelDiffrence = 1;
            float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;

            playerData.EarnXP(XPEarned);
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
            var score = GameController.Instance.Multiplier * baseEnemy.EnemyData.EnemyValue;
            var ultiplierTextToShow = GameController.Instance.Multiplier > 1 ? $"{score} + (x {GameController.Instance.Multiplier} )" : $"{score}";
            GuiManager.SetScoreMultipler(ultiplierTextToShow);
            GameController.Instance.NumberOfEnemies--;
            GameController.Instance.IncreaseMultiplier();

            GuiManager.CreateFloatingText("<color=" + "yellow" + ">" + XPEarned + "</color>" + "<color=" + "orange" + "> XP </color>", baseEnemy.transform.localPosition);

            playerData.SetPlayerKillsCounter(1);
            playerData.SetScore(score);

            if (baseEnemy.GetComponent<BossEnemy>() == null) return;
            playerData.SetBossKilledCount();
            gameInfo.BossBattleInitiated = false;

        }
    }

    public void OnEnemyEscapedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            if (baseEnemy.GetComponent<BossEnemy>()) return;
            GameController.Instance.EnemyEscaped++;
        }
    }

    public void OnEnemyHitHandled(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            var playerData = PersistantData.GetPlayerData();
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
        }
    }
}
//=================================================================================  