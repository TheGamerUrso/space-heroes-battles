using System.Collections;
using UnityEngine;

public class SurvivalGameMode : BaseGameMode
{
    private bool BossWave = false;
    GameObject enemGO = null;

    bool active = false;
    bool rewardToClaim = false;

    public override void SetGameMode()
    {
        base.SetGameMode();
        gameInfo.waves = 0;
        gameInfo.LevelDifficulty = playerData.GetCurrentPlayerShipData().level;
        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            SpawnPoints[i].enemyElements = gameInfo.enemyElements;
            SpawnPoints[i].LevelDifficulty = gameInfo.LevelDifficulty;
        }
    }

    public void NewWave()
    {
        BossWave = false;

        gameInfo.CurrentTotalEnemies = gameInfo.TotalEnemies;

        gameInfo.waves++;

        string[] transmitions = { "Wave:\n" + gameInfo.waves };
        GuiManager.PlayTrasmition(transmitions);

        if (gameInfo.waves > 0 && gameInfo.waves % 4 == 0)
        {
            gameInfo.availableEnemies++;

            if (gameInfo.availableEnemies > gameInfo.enemyElements.Count)
            {
                gameInfo.availableEnemies = gameInfo.enemyElements.Count;
            }

        }

        if (gameInfo.waves > 0 && gameInfo.waves % 2 == 0)
        {
            BossWave = true;
        }
    }

    public override IEnumerator UpdateGameMode()
    {
        if (GameController.CurrentGameState == GameController.GameState.START)
        {
            yield return new WaitUntil(() => (GameController.CurrentGameState == GameController.GameState.GAME));
        }

        yield return shortWait;
        playerShip = PlayerManager.GetPlayer();
        while (!Game.IsGameOver)
        {
            NewWave();

            if (GuiManager.Instance.IsTrasnmiting() || GameController.CurrentGameState == GameController.GameState.GAME)
            {
                yield return new WaitUntil(() => !GuiManager.Instance.IsTrasnmiting());
            }

            yield return shortWait;

            Game.UseSlowMo = true;

            if (gameInfo.pause)
            {
                yield return new WaitUntil(() => !gameInfo.pause);
            }

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

            if (Enemies.Count > 0)
            {
                yield return new WaitUntil(() => Enemies.Count <= 0);
            }

            yield return shortWait;

            if (playerShip.CurrentHealth > 0)
            {
                if (Enemies.Count > 0)
                {
                    yield return new WaitUntil(() => Enemies.Count <= 0);
                }

                if (BossWave)
                {
                    GuiManager.Instance.BossWarning();

                    yield return longWait;

                    SpawnBoss();

                    if (gameInfo.BossBattleInitiated)
                    {
                        yield return new WaitUntil(() => !gameInfo.BossBattleInitiated);


                        yield return RewardWait;

                        GuiManager.Instance.ShowRewardScreen();
                        rewardToClaim = true;

                        while (rewardToClaim)
                        {
                            rewardToClaim = Events.ClaimedReward();
                            yield return longWait;
                        }

                        yield return RewardWait;
                    }

                    active = true;
                }

                while (active)
                {
                    active = Events.HyperspaceEnded();
                    yield return null;
                }

                yield return longWait;
            }
        }
    }

    public override void Spawn()
    {
        gameInfo.CurrentTotalEnemies--;

        var random = Random.Range(0, SpawnPoints.Count);
        enemGO = SpawnPoints[random].SpawnEnemyElement(gameInfo.availableEnemies);
        Enemies.Add(enemGO);
    }

    public override void SpawnBoss()
    {
        if (!gameInfo.BossBattleInitiated)
        {
            gameInfo.BossBattleInitiated = true;

            currentBoss = BossFights[Random.Range(0, BossFights.Length)];
            SpawnBoss(currentBoss, gameInfo.LevelDifficulty);

            Enemies.Add(currentBoss.gameObject);
        }
    }
}

