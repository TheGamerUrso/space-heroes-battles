using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalMode : BaseGameMode
{
    [SerializeField] private GameObject[] BossFights;
    private bool BossWave = false;
    private GameObject BossPrefab;
    private bool FirstRun;

    public override void InitReference(PlayerData playerData, PlayerShip playerShip)
    {
        base.InitReference(playerData, playerShip);
        gameInfo.LevelDifficulty = playerData.GetCurrentPlayerShipData().level;
    }

    public override void Start()
    {
        base.Start();

        Game.IsSurvivalMode = true;

        gameInfo.waves = 0;

        playerShip = PlayerManager.GetPlayer();

        gameInfo.availableEnemies = level_SO.availableEnemies;
        gameInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;
        gameInfo.CurrentTotalEnemies = gameInfo.TotalEnemies;

        Game.EnemySpawnInTotal = gameInfo.TotalEnemies;

        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            SpawnPoints[i].enemyElements = gameInfo.enemyElements;
            SpawnPoints[i].LevelDifficulty = gameInfo.LevelDifficulty;
        }

        StartCoroutine(StartGameDelay());

    }

    public void NewWave()
    {
        BossWave = false;

        gameInfo.CurrentTotalEnemies = gameInfo.TotalEnemies;

        gameInfo.waves++;

        string[] transmitions = { "Wave:\n" + gameInfo.waves };
        GuiManager.PlayTrasmition(transmitions);

        if (gameInfo.waves > 0 && gameInfo.waves % 4 == 0)
        {
            gameInfo.availableEnemies++;

            if (gameInfo.availableEnemies > gameInfo.enemyElements.Count)
            {
                gameInfo.availableEnemies = gameInfo.enemyElements.Count;
            }

        }

        if (gameInfo.waves > 0 && gameInfo.waves % 2 == 0)
        {
            BossPrefab = BossFights[Random.Range(0, BossFights.Length)];
            BossWave = true;
        }
    }

    protected override IEnumerator StartGameDelay()
    {
        yield return CooldownTimer;

        NewWave();

        StartCoroutine(Spawn());
    }

    GameObject enemGO = null;

    bool active = false;
    bool rewardToClaim = false;

    public override IEnumerator Spawn()
    {
        while (!Game.IsGameOver)
        {
            if (GameController.CurrentGameState == GameController.GameState.START)
            {
                yield return new WaitUntil(() => (GameController.CurrentGameState == GameController.GameState.GAME));
            }

            yield return shortWait;

            if (GuiManager.Instance.IsTrasnmiting() || GameController.CurrentGameState == GameController.GameState.GAME)
            {
                yield return new WaitUntil(() => !GuiManager.Instance.IsTrasnmiting());
            }

            yield return shortWait;

            Game.UseSlowMo = true;

            if (gameInfo.pause)
            {
                yield return new WaitUntil(() => !gameInfo.pause);
            }

            while (gameInfo.CurrentTotalEnemies > 0)
            {
                while (gameInfo.pause)
                {
                    yield return shortWait;
                }

                gameInfo.CurrentTotalEnemies--;

                var random = Random.Range(0, SpawnPoints.Count);

                enemGO = SpawnPoints[random].SpawnEnemyElement(gameInfo.availableEnemies);
                Enemies.Add(enemGO);

                yield return CooldownTimer;
                CooldownTimer = new WaitForSeconds(cooldown);
            }

            if (Enemies.Count > 0)
            {
                yield return new WaitUntil(() => Enemies.Count <= 0);
            }

            yield return shortWait;

            if (playerShip.CurrentHealth > 0)
            {
                if (Enemies.Count > 0)
                {
                    yield return new WaitUntil(() => Enemies.Count <= 0);
                }

                if (BossWave)
                {
                    GuiManager.Instance.BossWarning();

                    yield return longWait;

                    if (!gameInfo.BossBattleInitiated)
                    {
                        gameInfo.BossBattleInitiated = true;

                        currentBoss = SpawnBoss(BossPrefab, gameInfo.LevelDifficulty);

                        Enemies.Add(currentBoss.gameObject);
                    }

                    if (gameInfo.BossBattleInitiated)
                    {
                        yield return new WaitUntil(() => !gameInfo.BossBattleInitiated);


                        yield return RewardWait;

                        GuiManager.Instance.ShowRewardScreen();
                        rewardToClaim = true;

                        while (rewardToClaim)
                        {
                            rewardToClaim = Events.ClaimedReward();
                            yield return longWait;
                        }

                        yield return RewardWait;
                    }

                    active = true;
                }



                while (active)
                {
                    active = Events.HyperspaceEnded();
                    yield return null;
                }

                yield return longWait;


                NewWave();

            }
        }
    }

    public override void BossDiedCallback(string id, BossEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            base.BossDiedCallback(id, baseEnemy);
        }
    }

}

