using System.Collections;
using System.Collections.Generic;
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
    public override void Start()
    {
        playerShip = PlayerManager.GetPlayer();

        enemyElements = level_SO.enemyElements;

        LevelDifficulty = level_SO.LevelDifficulty;

        TotalEnemies = level_SO.numberOfEnemiesEachWave * 2;

        for (int i = 0; i < availableEnemies; i++)
        {
            ListOfEnemyElements.Add(enemyElements[i].Name, enemyElements[i]);
        }


        availableEnemies = 1;

        level_SO.BossPrefab = BossFights[Random.Range(0, BossFights.Length)];

        StartCoroutine(StartGameDelay());

        Game.IsSurvivalMode = true;
    }


    public void NewWave()
    {
        waves++;

        HasBoss = false;

        var startingTotalEnemies = TotalEnemies;
        IncomingDanger = false;

        if (waves > 0 && waves % 2 == 0)
        {
            availableEnemies++;

            if (availableEnemies > enemyElements.Count)
            {
                availableEnemies = enemyElements.Count;
            }
        }

        if (waves > 0 && waves % 4 == 0)
        {
            HasBoss = true;
        }

        string[] transmitions = { "Wave:\n" + waves };
        GuiManager.PlayTrasmition(transmitions);

        TotalEnemies = level_SO.numberOfEnemiesEachWave * 2;
    }

    public override void StartGame()
    {
        string[] transmitions = { "Wave:\n" + waves };
        GuiManager.PlayTrasmition(transmitions);

        StartCoroutine(Spawn());
    }

    public override IEnumerator Spawn()
    {
        while (GameController.Instance.currentGameState == GameController.GameState.START)
        {
            yield return null;
        }

        var startingTotalEnemies = TotalEnemies;

        while (GameController.Instance.currentGameState == GameController.GameState.GAME)
        {
            while (GuiManager.Instance.IsTrasnmiting())
            {
                yield return waitForEndOfFrame;
            }

            while (TotalEnemies > 0 && !Game.IsGameOver)
            {
                var totalEnemiesPresetnage = startingTotalEnemies / TotalEnemies;

                if (totalEnemiesPresetnage < .5f)
                {
                    if (HasBoss && !IncomingDanger)
                    {
                        IncomingDanger = true;
                        GuiManager.PlayTrasmition(null, true);
                    }
                }

                int randomNumb = 0;

                availableEnemie = enemyElements.GetRange(0, availableEnemies);
                tempList = availableEnemie.Where(x => (x.currentNumberInScene < x.MaxNumberInScene)).ToList();
                var range = 0;

                for (int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i].presentage > 0f)
                    {
                        range += tempList[i].presentage;
                    }
                }

                var rand = Random.Range(0, range);
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
                waitForCooldown = new WaitForSeconds(cooldown);
                yield return waitForCooldown;


            }

            if (level_SO.HasBoss)
                GuiManager.PlayTrasmition(null, true);

            yield return waitForCooldown;

            while (Enemies.Count > 0)
            {
                yield return waitForCooldown;
            }

            if (level_SO.HasBoss)
            {
                if (!BossBattleInitiated)
                {
                    BossBattleInitiated = true;

                    currentBoss = SpawnEnemies.SpawnBoss(level_SO.BossPrefab, LevelDifficulty);

                    Enemies.Add(currentBoss);
                }


                while (BossBattleInitiated)
                {
                    yield return shortWait;
                }

                bool active = true;

                while (active)
                {
                    active = Events.OnWaveEnded();
                    yield return null;
                }

                if (AudioManager.Instance)
                    AudioManager.PlayRandomMusic(true);

                LevelDifficulty += 4;
            }

            NewWave();
        }

    }

    public override void BossDiedCallback(string id, BaseEnemy baseEnemy)
    {
        Enemies.Remove(baseEnemy.gameObject);

        TotalEnemies--;

        Events.EnemyDied?.Invoke(baseEnemy.Id, baseEnemy);

        int rand = Random.Range(4, 8);

        for (int i = 0; i < rand; i++)
        {
            DropController.PickRandomDropItem(baseEnemy.transform);
        }

        level_SO.BossPrefab = BossFights[Random.Range(0, BossFights.Length)];
        BossBattleInitiated = false;
        Destroy(baseEnemy.gameObject);
    }
}

