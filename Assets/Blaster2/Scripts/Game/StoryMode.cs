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

        gameInfo.LevelDifficulty = level_SO.LevelDifficulty;

        gameInfo.availableEnemies = level_SO.availableEnemies;
        gameInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;
        gameInfo.CurrentTotalEnemies = gameInfo.TotalEnemies;

        Game.EnemySpawnInTotal = gameInfo.TotalEnemies;

        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            SpawnPoints[i].enemyElements = gameInfo.enemyElements;
            SpawnPoints[i].LevelDifficulty = gameInfo.LevelDifficulty;
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

    public override IEnumerator Spawn()
    {
        var startingTotalEnemies = gameInfo.TotalEnemies;

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

        while (gameInfo.CurrentTotalEnemies > 0)
        {
            while (gameInfo.pause)
            {
                yield return shortWait;
            }

            gameInfo.CurrentTotalEnemies--;
       
            var random = Random.Range(0, SpawnPoints.Count);

            enemGO = SpawnPoints[random].SpawnEnemyElement();
            Enemies.Add(enemGO);

            yield return CooldownTimer;
            CooldownTimer = new WaitForSeconds(cooldown);
        }

        if (Enemies.Count > 0)
        {
            yield return new WaitUntil(() => Enemies.Count <= 0);
        }

        yield return shortWait;

        if (playerShip.CurrentHealth > 0)
        {
            
            if (level_SO.HasBoss)
            {
                GuiManager.Instance.BossWarning();

                yield return longWait;

                if (!gameInfo.BossBattleInitiated)
                {
                    gameInfo.BossBattleInitiated = true;

                    currentBoss = SpawnBoss(level_SO.BossPrefab, gameInfo.LevelDifficulty);

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


}







