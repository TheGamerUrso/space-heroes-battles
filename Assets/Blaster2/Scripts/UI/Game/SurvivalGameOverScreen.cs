using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalGameOverScreen : MonoBehaviour
{
    [SerializeField] private LeaderboardScreen leaderboardScreen;

    public void UpdateLeaderboards()
    {
       leaderboardScreen.LoadLeaderboard();
    }

    public void QuitButton()
    {
        GameManager.Instance.LoadMainenu();
        GetComponent<CanvasGroup>().interactable = false;
    }

    public void ReplayButton()
    {
        GameManager.Instance.ResetLevel();
        GetComponent<CanvasGroup>().interactable = false;
    }
}
