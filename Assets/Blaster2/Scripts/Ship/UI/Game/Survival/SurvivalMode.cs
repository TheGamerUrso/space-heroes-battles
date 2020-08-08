using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalMode : BaseGameMode
{
    [SerializeField] private GameObject[] BossFights;
    private bool BossWave = false;
    private GameObject BossPrefab;
    private bool FirstRun;
    public override void InitReference(PlayerData playerData, PlayerShip playerShip)
    {
        base.InitReference(playerData, playerShip);
        gameInfo.LevelDifficulty = playerData.GetCurrentPlayerShipData().level;
    }

    public override void Start()
    {
        base.Start();

        playerShip = PlayerManager.GetPlayer();

        Game.IsSurvivalMode = true;

        gameInfo.waves = 0;

        spawnInfo.enemyElements = level_SO.enemyElements.ToList();
        spawnInfo.availableEnemies = level_SO.availableEnemies;
        spawnInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;

        Game.EnemySpawnInTotal = spawnInfo.TotalEnemies;

        for (int i = 0; i < spawnInfo.enemyElements.Count; i++)
        {
            spawnInfo.enemyElements[i].currentNumberInScene = 0;
        }


        for (int i = 0; i < spawnInfo.enemyElements.Count; i++)
        {
            ListOfEnemyElements.Add(spawnInfo.enemyElements[i].Name, spawnInfo.enemyElements[i]);
        }

        StartCoroutine(StartGameDelay());
    }

    public void NewWave()
    {
        BossWave = false;

        spawnInfo.TotalEnemies = level_SO.numberOfEnemiesEachWave * level_SO.waves;

        for (int i = 0; i < spawnInfo.enemyElements.Count; i++)
        {
            spawnInfo.enemyElements[i].currentNumberInScene = 0;
        }

        gameInfo.waves++;

        string[] transmitions = { "Wave:\n" + gameInfo.waves };
        GuiManager.PlayTrasmition(transmitions);

        if (gameInfo.waves > 0 && gameInfo.waves % 2 == 0)
        {

            gameInfo.LevelDifficulty++;
            spawnInfo.availableEnemies++;

            if (spawnInfo.availableEnemies > spawnInfo.enemyElements.Count)
            {
                spawnInfo.availableEnemies = spawnInfo.enemyElements.Count;
            }

        }

        if (gameInfo.waves > 0 && gameInfo.waves % 4 == 0)
        {
            BossPrefab = BossFights[Random.Range(0, BossFights.Length)];
            BossWave = true;
        }
    }

    protected override IEnumerator StartGameDelay()
    {
        yield return CooldownTimer;

        NewWave();

        StartCoroutine(Spawn());
    }

    GameObject enemGO = null;
    EnemyElement previousSpawnEnemy = null;
    int repetition = 0;
    System.Random r;
    bool active = false;
    bool rewardToClaim = false;

    public override IEnumerator Spawn()
    {
        r = new System.Random();

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

            if (playerShip.CurrentHealth > 0)
            {
                if (Enemies.Count > 0)
                {
                    yield return new WaitUntil(() => Enemies.Count <= 0);
                }

                if (BossWave)
                {
                    GuiManager.PlayTrasmition(null, true);

                    yield return longWait;

                    if (!gameInfo.BossBattleInitiated)
                    {
                        gameInfo.BossBattleInitiated = true;

                        currentBoss = SpawnEnemies.SpawnBoss(BossPrefab, gameInfo.LevelDifficulty);

                        Enemies.Add(currentBoss.gameObject);
                    }

                    if (gameInfo.BossBattleInitiated)
                    {
                        yield return new WaitUntil(() => !gameInfo.BossBattleInitiated);
                    }

                    active = true;
                }

                yield return longWait;

                GuiManager.Instance.ShowRewardScreen();
                rewardToClaim = true;

                while (rewardToClaim)
                {
                    rewardToClaim = Events.ClaimedReward();
                    yield return longWait;
                }

                yield return longWait;

                while (active)
                {
                    active = Events.HyperspaceEnded();
                    yield return null;
                }

                yield return longWait;

                NewWave();
            }
        }
    }

    public override void BossDiedCallback(string id, BaseEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            base.BossDiedCallback(id, baseEnemy);

            int rand = Random.Range(4, 8);

            for (int i = 0; i < rand; i++)
            {
                DropController.PickRandomDropItem(baseEnemy.transform);
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
                        Debug.Log(previousSpawnEnemy.gameObjectType.ToString() + " " + repetition);
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

