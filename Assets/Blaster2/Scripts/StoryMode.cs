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

        GameSession.EnemySpawnInTotal = TotalEnemies;

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
        while(GameController.Instance.currentGameState == GameController.GameState.START)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        // Debug.Log("Game Started");
        waitForSec = new WaitForSeconds(delay);
        waitForCooldown = new WaitForSeconds(cooldown);

        var startingTotalEnemies = TotalEnemies;
        bool IncomingDanger = false;

        while (GameController.Instance.currentGameState == GameController.GameState.GAME)
        {
            while (GuiManager.Instance.IsTrasnmiting() || GameController.Instance.currentGameState != GameController.GameState.GAME)
            {
                yield return waitForEndOfFrame;
            }

            while (TotalEnemies > 0 && !GameEnded)
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

                int randomNumb = 0;

                availableEnemie = enemyElements.GetRange(0, availableEnemies);


                var range = 0;
                do
                {
                    tempList = availableEnemie.Where(
       x => (x.currentNumberInScene < x.MaxNumberInScene && x.presentage > 0)).ToList();
                    yield return null;
                } while (tempList.Count == 0);

                for (int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i].presentage > 0f)
                    {
                        range += tempList[i].presentage;
                    }
                }

                var rand = UnityEngine.Random.Range(0, range);
                var top = 0;

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

                while (pause)
                {
                    yield return waitForEndOfFrame;
                }


                if (TotalEnemies - 1 >= 0)
                {
                    GameObject enemGO = SpawnEnemies.SpawnEnemyElement(enemyElement);
                    Enemies.Add(enemGO);
                }

                yield return waitForCooldown;
            }

            if (playerShip.CurrentHealth > 0)
            {
                while (Enemies.Count > 0)
                {
                    yield return waitForCooldown;
                }

                if (HasBoss)
                {
                    if (!BossBattleInitiated)
                    {
                        BossBattleInitiated = true;

                        currentBoss = SpawnEnemies.SpawnBoss(BossPrefab, LevelDifficulty);

                        Enemies.Add(currentBoss);
                    }


                    while (BossBattleInitiated)
                    {
                        yield return waitforOneSec;
                    }
                }
                else
                {
                    GameOver();
                }


                yield return waitForFourSeconds;

                if (!GameSession.IsGameOver)
                {
                    Events.GameEnded?.Invoke();
                }
            }
        }
    }
}


