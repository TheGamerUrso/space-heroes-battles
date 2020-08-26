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
        AudioManager.PlayMusic(((LevelEnum)scene.buildIndex).ToString());

        playerShip = PlayerManager.GetPlayer();

        spawnInfo.enemyElements = level_SO.enemyElements.ToList();

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

        string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);

        StartCoroutine(Spawn());
    }

    GameObject enemGO = null;

    public override IEnumerator Spawn()
    {
        var startingTotalEnemies = spawnInfo.TotalEnemies;

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
                    Debug.Log(l_enemy + "=" + lowLim + ":" + random + ":" + hiLim);
                    enemyElement = availableEnemie[l_enemy];
                    break;
                }
            }


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
        if (availableEnemie.Count > 0)
        {
            int randEnemyIndex = UnityEngine.Random.Range(0, availableEnemie.Count);
            enemyElement = availableEnemie[randEnemyIndex];
        }
    }
}







