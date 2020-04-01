using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WinWidget : MonoBehaviour
{
    private PlayerShip player;
    private PlayerData playerData;
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
        //TODO Get PlayerShip

        levelName = "Level" + GameManager.LevelSelected;

        //TODO EnemySPawnInTotal * 9f
        //TODO CoinsDropInTotal * .9f;

        player = PlayerManager.GetPlayer();
        playerData = DataController.GetPlayerData();

        playerData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;

        missionCollection = DataController.GetMissionCollection();
        mission = missionCollection.GetMission(GameManager.LevelSelected);

        levelObjectiveDatas = playerData.GetLevelObjectives(levelName);

        if (levelObjectiveDatas[0].completed == false)
        {
            levelObjectiveDatas[0].completed = true;

            player.AddXP(50);
           // Debug.Log("Challenge : Complete the Mission (100 XP Awarded)");
        }

        //TODO Enemy Killed This Round
        float enemyKilled = 0;
        
        if (!levelObjectiveDatas[1].completed && enemyKilled >= killed)
        {
            levelObjectiveDatas[1].completed = true;
            player.AddXP(75);
        }

        if (!levelObjectiveDatas[2].completed && playerData.PlayedGame && player.IsPlayerDamaged == false)
        {
            levelObjectiveDatas[2].completed = true;
            player.AddXP(100);
           // Debug.Log("Challenge : Do Not Get Hit Completed (100 XP Awarded)");
        }

        float coinEarnInGame = 0;

        if (!levelObjectiveDatas[3].completed && coinEarnInGame >= 0 && coinEarnInGame >= collected)
        {
            levelObjectiveDatas[3].completed = true;
            player.AddXP(25);
           // Debug.Log("Challenge : Earn " + SpawnEnemies.CoinDropInTotal * .9f + " Completed" + "(100 XP Awarded)");
        }



        StartCoroutine(ShowGameResults());
    }

    private IEnumerator ShowGameResults()
    {
        //TODO Get Score
        float score = 0;
        string scoreText = string.Format("{00:0000000000}", score);
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