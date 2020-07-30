using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryMode : BaseGameMode
{
    WaitForSeconds waitForCooldown = new WaitForSeconds(1);
    WaitForSeconds waitForFourSeconds = new WaitForSeconds(4);
    WaitForSeconds waitForSec;

    public override void Start()
    {
        base.Start();

        MissionCollection missionCollection = PersistantData.GetMissionCollection();


        for (int i = 0; i < spawnInfo.enemyElements.Count; i++)
        {
            spawnInfo.enemyElements[i].currentNumberInScene = 0;
        }

        Scene scene = SceneManager.GetActiveScene();
        int levelMission = scene.buildIndex - (int)LevelEnum.Level1;
        Mission mission = missionCollection.GetMission(levelMission);


        playerShip = PlayerManager.GetPlayer();

        spawnInfo.enemyElements = level_SO.enemyElements.ToList();
        gameInfo.LevelDifficulty = mission.Level;

        spawnInfo.availableEnemies = level_SO.availableEnemies;
        spawnInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;

        Game.EnemySpawnInTotal = spawnInfo.TotalEnemies;

        for (int i = 0; i < spawnInfo.availableEnemies; i++)
        {
            ListOfEnemyElements.Add(spawnInfo.enemyElements[i].Name, spawnInfo.enemyElements[i]);
        }


        StartCoroutine(StartGameDelay());
    }


    public override void StartGame()
    {
        string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);

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

                if (level_SO.HasBoss)
                {
                    GuiManager.PlayTrasmition(null, true);

                    yield return new WaitForSeconds(2.0f);

                    Debug.Log("Boss Battle");
                    if (!gameInfo.BossBattleInitiated)
                    {
                        gameInfo.BossBattleInitiated = true;

                        currentBoss = SpawnEnemies.SpawnBoss(level_SO.BossPrefab,gameInfo.LevelDifficulty);

                        Enemies.Add(currentBoss);
                    }

                    if (gameInfo.BossBattleInitiated)
                    {
                        yield return new WaitUntil(() => !gameInfo.BossBattleInitiated);
                    }
                }


                yield return waitForFourSeconds;

                if (!Game.IsGameOver)
                {
                    Events.GameEnded?.Invoke();
                    Game.UseSlowMo = false;
                }
            }

        }
    }


}


