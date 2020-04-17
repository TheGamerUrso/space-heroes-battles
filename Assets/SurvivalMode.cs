using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalMode : SpawnEnemies
{
    public delegate bool WaveEnded();
    public WaveEnded OnWaveEnded;

    public GameObject[] BossFights;

    public override void OnStart()
    {
        playerShip = PlayerManager.GetPlayer();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetActiveScene().name.Equals("Gameplay"))
            {
                Debug.Log("Gameplay Scene active");
                continue;
            }
            Debug.Log("Not Gameplay Scene active");
        }

        LevelDifficulty = 1;

        string[] transmitions = { "Wave:\n" + waves, "Survive", "Good Luck" };
        GuiManager.PlayTrasmition(transmitions);

        TotalEnemies = numberOfEnemiesEachWave * 2;

        for (int i = 0; i < availableEnemies; i++)
        {
            ListOfEnemyElements.Add(enemyElements[i].Name, enemyElements[i]);
        }


        availableEnemies = 1;

        BossPrefab = BossFights[Random.Range(0, BossFights.Length)];

        StartCoroutine(Endless());
    }

    public void NewWave()
    {
        waves++;

        HasBoss = false;

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

        TotalEnemies = numberOfEnemiesEachWave * 2;
    }

    IEnumerator Endless()
    {
        Debug.Log("Game Started");
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        WaitForSeconds waitForSec = new WaitForSeconds(delay);
        WaitForSeconds waitForCooldown = new WaitForSeconds(cooldown);
        WaitForSeconds waitforOneSec = new WaitForSeconds(1);
        WaitForSeconds waitForFourSeconds = new WaitForSeconds(4);

        while (!GameEnded)
        {
            while (GuiManager.Instance.IsTrasnmiting())
            {
                yield return waitForEndOfFrame;
            }

            while (TotalEnemies > 0 && !GameEnded)
            {
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

                // randomNumb = UnityEngine.Random.Range(0, tempList.Count); 

                int repeat = 1;

                if (randomNumb == 0)
                {
                    repeat = UnityEngine.Random.Range(5, 8);
                }

                for (int i = 0; i < repeat; i++)
                {
                    if (TotalEnemies - 1 >= 0)
                    {
                        SpawnEnemyElement(enemyElement);
                    }
                    yield return waitForSec;
                }

                yield return waitForCooldown;


            }

            if (HasBoss)
                GuiManager.PlayTrasmition(null, true);

            yield return waitForCooldown;

            while (Enemies.Count > 0)
            {
                yield return waitForCooldown;
            }

            if (HasBoss)
            {
                if (!BossBattleInitiated)
                {
                    BossBattleInitiated = true;

                    Debug.Log("Boss Battle");
                    SpawnBoss();
                }


                while (BossBattleInitiated)
                {
                    yield return waitforOneSec;
                }

                bool active = true;

                while (active)
                {
                    active = OnWaveEnded();
                    Debug.Log(active);
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

        EnemyDied?.Invoke(baseEnemy);

        int rand = UnityEngine.Random.Range(4, 8);

        for (int i = 0; i < rand; i++)
        {
            DropController.PickRandomDropItem(baseEnemy.transform);
        }

        BossPrefab = BossFights[Random.Range(0, BossFights.Length)];
        BossBattleInitiated = false;
        Destroy(baseEnemy.gameObject);
    }
}

