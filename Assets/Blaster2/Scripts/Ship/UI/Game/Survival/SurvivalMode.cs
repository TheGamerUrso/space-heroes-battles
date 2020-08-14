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

        spawnInfo.enemyElements = level_SO.enemyElements;
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
        AudioManager.PlayRandomMusic(true);

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
        yield return waitForCooldown;

        NewWave();

        StartCoroutine(Spawn());
    }

    public override IEnumerator Spawn()
    {
        GameObject enemGO;

        waitForSec = new WaitForSeconds(delay);
        waitForCooldown = new WaitForSeconds(cooldown);

        var repetition = 0;
        var active = false;
        var rewardToClaim = false;

        EnemyElement previousSpawnEnemy = null;

        if ((GameController.Instance.currentGameState == GameController.GameState.START))
        {
            yield return new WaitUntil(() => (GameController.Instance.currentGameState == GameController.GameState.GAME));
        }

        yield return new WaitForSeconds(1.0f);


        while (!Game.IsGameOver)
        {
            if (GuiManager.Instance.IsTrasnmiting() || GameController.Instance.currentGameState == GameController.GameState.GAME)
            {
                yield return new WaitUntil(() => !GuiManager.Instance.IsTrasnmiting());
            }

            Game.UseSlowMo = true;

            while (spawnInfo.TotalEnemies > 0 && !Game.IsGameOver)
            {
                availableEnemie = spawnInfo.enemyElements.GetRange(0, spawnInfo.availableEnemies);

                var randomNumb = 0;
                var range = 0;
                var top = 0;

                do
                {
                    tempList = availableEnemie.Where(x => (x.currentNumberInScene < x.MaxNumberInScene && x.presentage > 0)).ToList();
                    yield return null;
                } while (tempList.Count == 0);

                for (int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i].presentage > 0f)
                    {
                        range += tempList[i].presentage;
                    }
                }

                var rand = Random.Range(0, range);

                for (int i = 0; i < tempList.Count; i++)
                {
                    top += tempList[i].presentage;
                    if (rand < top)
                    {
                        enemyElement = tempList[i];
                        randomNumb = i;

                        if (tempList.Count > 1)
                        {
                            if (previousSpawnEnemy != null)
                            {
                                if (previousSpawnEnemy.gameObjectType == enemyElement.gameObjectType)
                                {
                                    repetition++;
                                }
                                else if (previousSpawnEnemy.gameObjectType != enemyElement.gameObjectType)
                                {
                                    repetition = 0;
                                }
                            }
                        }

                        break;
                    }
                }

                if (gameInfo.pause)
                {
                    yield return new WaitUntil(() => !gameInfo.pause);
                }

                if (repetition <= 3 && tempList.Count >= 1)
                {
                    if (spawnInfo.TotalEnemies - 1 >= 0)
                    {
                        enemGO = SpawnEnemies.SpawnEnemyElement(GetEnemyElemeny(enemyElement.Name), gameInfo.LevelDifficulty);
                        Enemies.Add(enemGO);
                        previousSpawnEnemy = enemyElement;
                    }
                    yield return waitForCooldown;
                }
            }

            if (playerShip.CurrentHealth > 0)
            {
                if (Enemies.Count > 0)
                {
                    yield return new WaitUntil(() => Enemies.Count <= 0);
                }

                if (BossWave)
                {
                    GuiManager.PlayTrasmition(null, true);

                    yield return new WaitForSeconds(2.0f);

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
            }

            GuiManager.Instance.ShowRewardScreen();

            rewardToClaim = true;

            while (rewardToClaim)
            {
                rewardToClaim = Events.ClaimedReward();
                yield return null;
            }

            yield return waitForFourSeconds;

            while (active)
            {
                active = Events.HyperspaceEnded();
                yield return null;
            }

            NewWave();

        }
    }

    public override void BossDiedCallback(string id, BaseEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            base.BossDiedCallback(id, baseEnemy);

            int rand = Random.Range(4, 8);

            for (int i = 0; i < rand; i++)
            {
                DropController.PickRandomDropItem(baseEnemy.transform);
            }
        }
    }
}

