using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryMode : BaseGameMode
{
    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    WaitForSeconds waitForSec = new WaitForSeconds(1);
    WaitForSeconds waitForCooldown = new WaitForSeconds(1);
    WaitForSeconds waitforOneSec = new WaitForSeconds(1);
    WaitForSeconds waitForFourSeconds = new WaitForSeconds(4);

    public override void Start()
    {
        playerShip = PlayerManager.GetPlayer();

        MissionCollection missionCollection = PersistantData.GetMissionCollection();

        Scene scene = SceneManager.GetActiveScene();
        string index = scene.name[scene.name.Length - 1].ToString();
        Mission mission = missionCollection.GetMission(int.Parse(index));
        LevelDifficulty = mission.Level;

        TotalEnemies = numberOfEnemiesEachWave * waves;

        Game.EnemySpawnInTotal = TotalEnemies;

        for (int i = 0; i < availableEnemies; i++)
        {
            ListOfEnemyElements.Add(enemyElements[i].Name, enemyElements[i]);
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

        var startingTotalEnemies = TotalEnemies;
        bool IncomingDanger = false;
        int randomNumb = 0;
        var range = 0;
        var rand = Random.Range(0, range);
        var top = 0;

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

            while (TotalEnemies > 0 && !Game.IsGameOver)
            {
                float totalEnemiesPresetnage = (float)TotalEnemies / (float)startingTotalEnemies;
                if (totalEnemiesPresetnage < .1f)
                {
                    if (HasBoss && !IncomingDanger)
                    {
                        IncomingDanger = true;
                        GuiManager.PlayTrasmition(null, true);
                    }
                }                

                availableEnemie = enemyElements.GetRange(0, availableEnemies);
             
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

                for (int i = 0; i < tempList.Count; i++)
                {
                    top += tempList[i].presentage;
                    if (rand < top)
                    {
                        enemyElement = tempList[i];
                        randomNumb = i;
                        break;
                    }
                }

                if (pause)
                {
                    yield return new WaitUntil(() => !pause);
                }

                if (TotalEnemies - 1 >= 0)
                {
                    enemGO = SpawnEnemies.SpawnEnemyElement(enemyElement);
                    Enemies.Add(enemGO);
                }

                yield return waitForCooldown;
            }

            if (playerShip.CurrentHealth > 0)
            {
                if (Enemies.Count > 0)
                {
                    yield return new WaitUntil(() => Enemies.Count <= 0);
                }

                if (HasBoss)
                {
                    Debug.Log("Boss Battle");
                    if (!BossBattleInitiated)
                    {
                        BossBattleInitiated = true;

                        currentBoss = SpawnEnemies.SpawnBoss(BossPrefab, LevelDifficulty);

                        Enemies.Add(currentBoss);
                    }

                    if (BossBattleInitiated)
                    {
                        yield return new WaitUntil(() => !BossBattleInitiated);
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


