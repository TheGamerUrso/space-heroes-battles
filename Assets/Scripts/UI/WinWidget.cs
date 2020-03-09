using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WinWidget : MonoBehaviour
{
    private Player player;
    private PlayerData playerData;
    private GameController gameController;
    [SerializeField] private MissionCollection missionCollection;
    [SerializeField] private Mission mission;
    [SerializeField] private TextMeshProUGUI Score = null;
    private string levelName;
    private float killed;
    private float collected;

    [Header("Win Widget Config")]
    public LevelObjectivesElement[] levelObjectives;
    public LevelObjectiveData[] levelObjectiveDatas;

    public AudioClip audioClip;
    public AudioSource audioSource;

    public void PlaySound()
    {
        audioSource.PlayOneShot(audioClip);
    }

    private void OnEnable()
    {
        if (gameController==null)
        {
            gameController = GameController.Instance;
        }
        levelName = "Level" + GameManager.LevelSelected;
        killed = gameController.EnemySpawnedInTotal * .9f;
        collected = gameController.CoinDropInTotal * .9f;

        player = PlayerManager.GetPlayer();
        playerData = DataController.GetPlayerData();

        playerData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;

        missionCollection = DataController.GetMissionCollection();
        mission = missionCollection.GetMission(GameManager.LevelSelected);

        levelObjectiveDatas = playerData.GetLevelObjectives(levelName);

        if (levelObjectiveDatas[0].completed == false)
        {
            levelObjectiveDatas[0].completed = true;

            PlayerManager.GetPlayer().GetLevelSystem().AddXP(50);
           // Debug.Log("Challenge : Complete the Mission (100 XP Awarded)");
        }

        if (!levelObjectiveDatas[1].completed && gameController.EnemyKilled >= killed)
        {
            levelObjectiveDatas[1].completed = true;
            PlayerManager.GetPlayer().GetLevelSystem().AddXP(75);
           // Debug.Log("Challenge : Kill " + EnemyManager.EnemySpawnedInTotal * .9f + " Enemies Completed" + "(100 XP Awarded)");
        }

        if (!levelObjectiveDatas[2].completed && playerData.PlayedGame && player.IsPlayerDamaged() == false)
        {
            levelObjectiveDatas[2].completed = true;
            PlayerManager.GetPlayer().GetLevelSystem().AddXP(100);
           // Debug.Log("Challenge : Do Not Get Hit Completed (100 XP Awarded)");
        }

        if (!levelObjectiveDatas[3].completed && gameController.counsEarnInGame >= 0 && gameController.counsEarnInGame >= collected)
        {
            levelObjectiveDatas[3].completed = true;
            PlayerManager.GetPlayer().GetLevelSystem().AddXP(25);
           // Debug.Log("Challenge : Earn " + SpawnEnemies.CoinDropInTotal * .9f + " Completed" + "(100 XP Awarded)");
        }



        StartCoroutine(ShowGameResults());
    }

    private IEnumerator ShowGameResults()
    {
        string scoreText = string.Format("{00:0000000000}", gameController.Score);
        Score.text = scoreText;
        int ChallengeIndex = 0;

        foreach (LevelObjectivesElement item in levelObjectives)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < levelObjectives.Length; i++)
        {
            levelObjectives[i].levelObjectiveData = levelObjectiveDatas[i];
        }

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        ChallengeIndex = 1;

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        ChallengeIndex = 2;

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        ChallengeIndex = 3;

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();
    }
}