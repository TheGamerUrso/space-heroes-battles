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

        if (gameInfo.waves > 0 && gameInfo.waves % 2 == 0)
        {

            gameInfo.LevelDifficulty++;
            spawnInfo.availableEnemies++;

            if (spawnInfo.availableEnemies > spawnInfo.enemyElements.Count)
            {
                spawnInfo.availableEnemies = spawnInfo.enemyElements.Count;
            }

        }

        if (gameInfo.waves > 0 && gameInfo.waves % 4 == 0)
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
            var repeat = 1;

            for (int enemyIndex = 0; enemyIndex <= spawnInfo.TotalEnemies; enemyIndex++)
            {
                if (gameInfo.pause)
                {
                    yield return new WaitUntil(() => !gameInfo.pause);
                }

                while (Game.NumberOfEnemies >= MaxNumberOfEnemies)
                {
                    yield return new WaitForSeconds(.5f);
                }

                var random = Random.Range(0, 100); // draw a number between 0 and 99
                int lowLim;    // lowLim and hiLim are automatically set for each enemy
                int hiLim = 0;
                for (int l_enemy = 0; l_enemy < availableEnemie.Count; l_enemy++)
                {
                    lowLim = hiLim; // set low limit...
                    hiLim += availableEnemie[l_enemy].presentage; // and high limit
                    if (random >= lowLim && random < hiLim)
                    { // instantiate it!
                       // Debug.Log(l_enemy + "=" + lowLim + ":" + random + ":" + hiLim);
                        enemyElement = availableEnemie[l_enemy];
                        break;
                    }
                }


                if (spawnInfo.TotalEnemies - 1 >= 0)
                {
                    if (enemyElement != null)
                    {
                        if (enemyElement.gameObjectType == PoolGameObjectType.Enemy1)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (enemyIndex + 1 < spawnInfo.TotalEnemies)
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
                        Game.NumberOfEnemies++;
                    }
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
                    }

                    active = true;
                }

                yield return longWait;

                GuiManager.Instance.ShowRewardScreen();
                rewardToClaim = true;

                while (rewardToClaim)
                {
                    rewardToClaim = Events.ClaimedReward();
                    yield return longWait;
                }

                yield return longWait;

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

