using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalMode : BaseGameMode
{
    public GameObject[] BossFights;
    private bool IncomingDanger = false;
    public bool HasBoss = false;

    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    WaitForSeconds waitForCooldown = new WaitForSeconds(2);
    WaitForSeconds shortWait = new WaitForSeconds(1);
    WaitForSeconds waitForFourSeconds = new WaitForSeconds(4);
    WaitForSeconds waitForSec;

    public override void Start()
    {
        base.Start();

        Game.IsSurvivalMode = true;

        gameInfo.waves = level_SO.waves;

        level_SO.BossPrefab = BossFights[Random.Range(0, BossFights.Length)];

        playerShip = PlayerManager.GetPlayer();

        spawnInfo.enemyElements = level_SO.enemyElements;
        gameInfo.LevelDifficulty = level_SO.LevelDifficulty;

        spawnInfo.availableEnemies = level_SO.availableEnemies;
        spawnInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;

        Game.EnemySpawnInTotal = spawnInfo.TotalEnemies;


        for (int i = 0; i < spawnInfo.availableEnemies; i++)
        {
            ListOfEnemyElements.Add(spawnInfo.enemyElements[i].Name, spawnInfo.enemyElements[i]);
        }

        StartCoroutine(StartGameDelay());


    }


    public void NewWave()
    {
        gameInfo.waves = 4;

        HasBoss = false;

        var startingTotalEnemies = spawnInfo.TotalEnemies;
        IncomingDanger = false;

        if (gameInfo.waves > 0 && gameInfo.waves % 2 == 0)
        {
            //spawnInfo.availableEnemies++;

            spawnInfo.availableEnemies = 6;

            if (spawnInfo.availableEnemies > spawnInfo.enemyElements.Count)
            {
                spawnInfo.availableEnemies = spawnInfo.enemyElements.Count;
            }
        }

        if (gameInfo.waves > 0 && gameInfo.waves % 4 == 0)
        {
            HasBoss = true;
        }

        string[] transmitions = { "Wave:\n" + gameInfo.waves };
        GuiManager.PlayTrasmition(transmitions);

        spawnInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;
    }

    public override void StartGame()
    {
        NewWave();
        StartCoroutine(Spawn());
    }

    public override IEnumerator Spawn()
    {
        GameObject enemGO;

        waitForSec = new WaitForSeconds(delay);
        waitForCooldown = new WaitForSeconds(cooldown);

        var startingTotalEnemies = spawnInfo.TotalEnemies;
        var randomNumb = 0;
        var range = 0;
        var top = 0;
        var repetition = 0;
        var active = true;
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

                if (repetition <= 3 || tempList.Count > 1)
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

                if (HasBoss)
                {
                    GuiManager.PlayTrasmition(null, true);

                    yield return new WaitForSeconds(2.0f);

                    Debug.Log("Boss Battle");
                    if (!gameInfo.BossBattleInitiated)
                    {
                        gameInfo.BossBattleInitiated = true;

                        currentBoss = SpawnEnemies.SpawnBoss(level_SO.BossPrefab, gameInfo.LevelDifficulty);

                        Enemies.Add(currentBoss);
                    }

                    if (gameInfo.BossBattleInitiated)
                    {
                        yield return new WaitUntil(() => !gameInfo.BossBattleInitiated);
                    }
                }


                yield return waitForFourSeconds;

                active = true;

                active = Events.OnWaveEnded();

                yield return new WaitUntil(() => !active);

                AudioManager.PlayRandomMusic(true);

                gameInfo.LevelDifficulty += 4;

                NewWave();
            }

        }
    }

    public override void BossDiedCallback(string id, BaseEnemy baseEnemy)
    {
        Enemies.Remove(baseEnemy.gameObject);

        spawnInfo.TotalEnemies--;

        Events.EnemyDied?.Invoke(baseEnemy.Id, baseEnemy);

        int rand = Random.Range(4, 8);

        for (int i = 0; i < rand; i++)
        {
            DropController.PickRandomDropItem(baseEnemy.transform);
        }

        level_SO.BossPrefab = BossFights[Random.Range(0, BossFights.Length)];
        gameInfo.BossBattleInitiated = false;
        Destroy(baseEnemy.gameObject);
    }
}

