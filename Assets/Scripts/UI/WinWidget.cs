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

   public void ShowGameResult()
    {
        //TODO Get PlayerShip

        levelName = "Level" + GameManager.LevelSelected;

        if (levelName.Equals("Level0"))
        {
            return;
        }

        //TODO EnemySPawnInTotal * 9f
        //TODO CoinsDropInTotal * .9f;

        player = PlayerManager.GetPlayer();
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();

        playerShipData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;

        levelName = "Level" + GameManager.LevelSelected;

        if (levelName.Equals("Level0"))
        {
            return;
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