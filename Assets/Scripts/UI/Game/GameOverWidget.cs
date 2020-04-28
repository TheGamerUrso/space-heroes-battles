using EasyMobile;
using System.Collections;
using TheGamerUrso.SceneLoader;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWidget : MonoBehaviour
{
    protected PlayerShipElement player;
    protected PlayerData playerData;

    public TextMeshProUGUI m_Text;
    public Button m_PlayAgainButton;
    public Button m_QuitButton;
    public GameObject m_Canvas;

    [Header("GameOver Widget Config")]
    public LevelObjectivesElement[] levelObjectives;
    private LevelObjectiveData[] levelObjectiveDatas;

    public void ShowGameResult()
    {
        float score = GameSession.score;
        string scoreText = string.Format("{00:0000000000}", score);
        m_Text.text = scoreText;

        if (GameManager.Instance == null)
        {
            return;
        }

        string levelName = "Level" + (GameManager.LevelIndexSelected + 1);

        if (levelName.Equals("Level0"))
        {
            return;
        }

        playerData = PersistantData.GetPlayerData();
        levelObjectiveDatas = playerData.GetLevelObjectives(levelName);

        StartCoroutine(ShowGameResults());
    }

    private IEnumerator ShowGameResults()
    {
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

    public void ReplayButton()
    {
        SceneLoader.Instance.ResetLevel();
    }
    public void LoadMainMenu()
    {
        GuiManager.Instance.LoadMainMenu();
    }
}