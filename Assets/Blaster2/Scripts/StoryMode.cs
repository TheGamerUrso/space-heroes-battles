using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryMode : BaseGameMode
{
    protected override void Awake()
    {
        base.Awake();

        shortDelay = new WaitForSeconds(delay);
        CooldownTimer = new WaitForSeconds(cooldown);
    }

    public override void Start()
    {
        base.Start();

        MissionCollection missionCollection = PersistantData.GetMissionCollection();


        Scene scene = SceneManager.GetActiveScene();
        int levelMission = scene.buildIndex - (int)LevelEnum.Level1;
        Mission mission = missionCollection.GetMission(levelMission);


        playerShip = PlayerManager.GetPlayer();

        spawnInfo.enemyElements = level_SO.enemyElements.ToList();
        gameInfo.LevelDifficulty = mission.Level;

        spawnInfo.availableEnemies = level_SO.availableEnemies;
        spawnInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;

        Game.EnemySpawnInTotal = spawnInfo.TotalEnemies;

        for (int i = 0; i < spawnInfo.enemyElements.Count; i++)
        {
            spawnInfo.enemyElements[i].currentNumberInScene = 0;
        }

        for (int i = 0; i < spawnInfo.availableEnemies; i++)
        {
            ListOfEnemyElements.Add(spawnInfo.enemyElements[i].Name, spawnInfo.enemyElements[i]);
        }


        StartCoroutine(StartGameDelay());
    }

    protected override IEnumerator StartGameDelay()
    {
        yield return CooldownTimer;

        string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);

        StartCoroutine(Spawn());
    }

    GameObject enemGO = null;
    EnemyElement previousSpawnEnemy = null;
    int repetition = 0;
    System.Random r;

    public override IEnumerator Spawn()
    {
        r = new System.Random();
        var startingTotalEnemies = spawnInfo.TotalEnemies;

        while (!Game.IsGameOver)
        {
            if ((GameController.Instance.currentGameState == GameController.GameState.START))
            {
                yield return new WaitUntil(() => (GameController.Instance.currentGameState == GameController.GameState.GAME));
            }

            yield return shortWait;

            if (GuiManager.Instance.IsTrasnmiting() || GameController.Instance.currentGameState == GameController.GameState.GAME)
            {
                yield return new WaitUntil(() => !GuiManager.Instance.IsTrasnmiting());
            }

            yield return shortWait;

            Game.UseSlowMo = true;

            availableEnemie = spawnInfo.enemyElements.GetRange(0, spawnInfo.availableEnemies);
            availableEnemie.Reverse();

            while (spawnInfo.TotalEnemies > 0)
            {
                if (gameInfo.pause)
                {
                    yield return new WaitUntil(() => !gameInfo.pause);
                }

                ChooseRandomEnemyToSpawn();

                if (repetition <= 2)
                {
                    if (spawnInfo.TotalEnemies - 1 >= 0)
                    {
                        if (enemyElement != null)
                        {
                            enemGO = spawnEnemies.SpawnEnemyElement(GetEnemyElemeny(enemyElement.Name), gameInfo.LevelDifficulty);
                            Enemies.Add(enemGO);
                            previousSpawnEnemy = enemyElement;
                        }
                    }
                }

                yield return CooldownTimer;
                CooldownTimer = new WaitForSeconds(cooldown);
            }

            yield return shortWait;

            if (Game.IsGameOver)
            {
                break;
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

                yield return longWait;

                Debug.Log("Boss Battle");
                if (!gameInfo.BossBattleInitiated)
                {
                    gameInfo.BossBattleInitiated = true;

                    currentBoss = SpawnEnemies.SpawnBoss(level_SO.BossPrefab, gameInfo.LevelDifficulty);

                    Enemies.Add(currentBoss.gameObject);
                }

                if (gameInfo.BossBattleInitiated)
                {
                    yield return new WaitUntil(() => !gameInfo.BossBattleInitiated);
                }
            }


            yield return longWait;

            if (!Game.IsGameOver)
            {
                Events.GameEnded?.Invoke();
                Game.UseSlowMo = false;
            }
        }

    }

    public void ChooseRandomEnemyToSpawn()
    {
        tempList = availableEnemie.Where(x => (x.currentNumberInScene < x.MaxNumberInScene && x.presentage > 0)).ToList();

        if (tempList.Count > 0)
        {
            if (tempList.Count > 1)
            {
                double diceRoll = r.NextDouble() * 100;
                double cumulative = 0.0;
                
                for (int i = 0; i < tempList.Count; i++)
                {
                    cumulative += tempList[i].presentage;

                    if (diceRoll < cumulative)
                    {
                        enemyElement = tempList[i];
                        break;
                    }
                }
            }
            else
            {
                enemyElement = tempList[0];
            }

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
        }
    }
}







