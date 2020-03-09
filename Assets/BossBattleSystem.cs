using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] BossPrefab = null;
    public GameController gameController;

    private void Start()
    {
        gameController = GameController.Instance;
    }

    public void ShowBossFightWarning()
    {
        StartCoroutine(ShowWarning());
    }
    public void StartBossFight()
    {
        if (!gameController.BossStage)
        {
            StartCoroutine(BossFight());
        }
    }

    IEnumerator ShowWarning()
    {
        //AudioManager.PlaySound("Danger", 3);
        string[] transmitions = { "There is something Big Coming on your way", "Be Careful" };
        GuiManager.PlayTrasmition(transmitions, true);

        while (GuiManager.IsTrasnmiting())
        {
            GuiManager.CountdownVisibility(false);
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator BossFight()
    {
        gameController.BossStage = true;
        AudioManager.PlayMusic("Boss");
        GameObject bossprefab = BossPrefab[UnityEngine.Random.Range(0, BossPrefab.Length)];
        EnemyManager.Instance.SpawnBoss(bossprefab, gameController.LevelDifficulty);
        while (gameController.BossStage)
        {
            yield return new WaitForSeconds(1.0f);
        }
        yield return new WaitForSeconds(1.0f);
    }
}


