using TMPro;
using UnityEngine;

public class HighscoreScreen : MonoBehaviour
{
    private PlayerData playerData;
    public TextMeshProUGUI[] HighScore;

    private void Start()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        PlayerData playerData =  DataController.Instance.GetPlayerData();

        MissionCollection missionCollection = DataController.Instance.GetMissionCollection();

        int numberOfLevels = missionCollection.Missions.Length;

        for (int i = 0; i < HighScore.Length; i++)
        {
            float highscore = playerData.GetHighScore(i + 1);
            string scoreText = string.Format("{00:00000000}", highscore);
            HighScore[i].text = string.Format("Level {0} : {1}", missionCollection.Missions[i+1].ID, scoreText);
        }

    }
}
