using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryGameMode : BaseGameMode
{    
    GameObject enemGO = null;

    public override void SetGameMode()
    {
        base.SetGameMode();
        gameInfo.LevelDifficulty = level_SO.LevelDifficulty;
        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            SpawnPoints[i].enemyElements = gameInfo.enemyElements;
            SpawnPoints[i].LevelDifficulty = gameInfo.LevelDifficulty;
        }

        Scene scene = SceneManager.GetActiveScene();
        int levelMission = scene.buildIndex - (int)LevelEnum.Level1;
        AudioManager.PlayMusic(((LevelEnum)scene.buildIndex).ToString());
    }

    public override void Start()
    {
        base.Start();

        Scene scene = SceneManager.GetActiveScene();
        int levelMission = scene.buildIndex - (int)LevelEnum.Level1;
        AudioManager.PlayMusic(((LevelEnum)scene.buildIndex).ToString());

        StartCoroutine(StartGameDelay());
    }

    public override IEnumerator UpdateGameMode()
    {       
        string[] transmitions = { "Enemies Approaching", "Defeat them", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);
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

            Spawn();

            yield return CooldownTimer;
            CooldownTimer = new WaitForSeconds(cooldown);
        }

        if (EnemiesCount> 0)
        {
            yield return new WaitUntil(() => EnemiesCount <= 0);
        }

        yield return shortWait;

        if (PlayerManager.GetPlayer().CurrentHealth > 0)
        {
            
            if (level_SO.HasBoss)
            {
                GuiManager.Instance.BossWarning();

                yield return longWait;

                SpawnBoss();

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

    public override void Spawn()
    {
        EnemiesCount++;
        gameInfo.CurrentTotalEnemies--;
        var random = Random.Range(0, SpawnPoints.Count);
        enemGO = SpawnPoints[random].SpawnEnemyElement();
    }

    public override void SpawnBoss()
    {
        if (!gameInfo.BossBattleInitiated)
        {
            gameInfo.BossBattleInitiated = true;

            currentBoss = level_SO.BossPrefab;
            SpawnBoss(currentBoss, gameInfo.LevelDifficulty);

           EnemiesCount++;
        }
    }

}







