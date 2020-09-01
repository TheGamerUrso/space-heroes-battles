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

        playerShip = PlayerManager.GetPlayer();

        Game.IsSurvivalMode = true;

        gameInfo.waves = 0;

        spawnInfo.enemyElements = level_SO.enemyElements.ToList();
        spawnInfo.availableEnemies = level_SO.availableEnemies;
        spawnInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;
        gameInfo.LevelDifficulty = level_SO.LevelDifficulty;

        Game.EnemySpawnInTotal = spawnInfo.TotalEnemies;

        for (int i = 0; i < spawnInfo.enemyElements.Count; i++)
        {
            ListOfEnemyElements.Add(spawnInfo.enemyElements[i].Name, spawnInfo.enemyElements[i]);
        }

        StartCoroutine(StartGameDelay());

    }

    public void NewWave()
    {
        BossWave = false;

        spawnInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;

        for (int i = 0; i < spawnInfo.enemyElements.Count; i++)
        {
            spawnInfo.enemyElements[i].currentNumberInScene = 0;
        }

        gameInfo.waves++;

        string[] transmitions = { "Wave:\n" + gameInfo.waves };
        GuiManager.PlayTrasmition(transmitions);

        if (gameInfo.waves > 0 && gameInfo.waves % 4 == 0)
        {
            spawnInfo.availableEnemies++;

            if (spawnInfo.availableEnemies > spawnInfo.enemyElements.Count)
            {
                spawnInfo.availableEnemies = spawnInfo.enemyElements.Count;
            }

        }

        if (gameInfo.waves > 0 && gameInfo.waves % 2 == 0)
        {
            BossPrefab = BossFights[Random.Range(0, BossFights.Length)];
            BossWave = true;
        }
    }

    public float reachMaxEnemiesTimer = 2;

    public void Update()
    {
        if (reachMaxEnemiesTimer > 0)
        {
            reachMaxEnemiesTimer -= Time.deltaTime;
            if (reachMaxEnemiesTimer <= 0)
            {
                Game.NumberOfEnemies--;
                reachMaxEnemiesTimer = 2;
                if (Game.NumberOfEnemies <= 0)
                {
                    Game.NumberOfEnemies = 0;
                }
            }
        }
    }

    protected override IEnumerator StartGameDelay()
    {
        yield return CooldownTimer;

        NewWave();

        StartCoroutine(Spawn());
    }

    GameObject enemGO = null;
    EnemyElement previousSpawnEnemy = null;

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

            availableEnemie = spawnInfo.enemyElements.GetRange(0, spawnInfo.availableEnemies);

            for (int enemyIndex = 0; enemyIndex < spawnInfo.TotalEnemies; enemyIndex++)
            {
                if (gameInfo.pause)
                {
                    yield return new WaitUntil(() => !gameInfo.pause);
                }

                while (Game.NumberOfEnemies >= MaxNumberOfEnemies)
                {
                    yield return new WaitForSeconds(.5f);
                }

                var random = Random.Range(0, availableEnemie.Count);

                enemyElement = availableEnemie[random];

                if (enemyElement.gameObjectType == PoolGameObjectType.Enemy1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (enemyIndex + 1 <= spawnInfo.TotalEnemies)
                        {
                            enemGO = spawnEnemies.SpawnEnemyElement(enemyElement, gameInfo.LevelDifficulty);
                            Enemies.Add(enemGO);
                            enemyIndex++;
                            yield return new WaitForSeconds(.5f);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    enemGO = spawnEnemies.SpawnEnemyElement(enemyElement, gameInfo.LevelDifficulty);
                    Enemies.Add(enemGO);
                }

                yield return CooldownTimer;
                CooldownTimer = new WaitForSeconds(cooldown);
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
                    GuiManager.PlayTrasmition(null, true);

                    yield return longWait;

                    if (!gameInfo.BossBattleInitiated)
                    {
                        gameInfo.BossBattleInitiated = true;

                        currentBoss = SpawnEnemies.SpawnBoss(BossPrefab, gameInfo.LevelDifficulty);

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

    public void ChooseRandomEnemyToSpawn()
    {
        if (availableEnemie.Count > 0)
        {
            int randEnemyIndex = UnityEngine.Random.Range(0, availableEnemie.Count);
            enemyElement = availableEnemie[randEnemyIndex];
        }
    }
}

