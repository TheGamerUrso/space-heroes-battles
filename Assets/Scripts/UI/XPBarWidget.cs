using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPBarWidget : MonoBehaviour
{
    public TextMeshProUGUI PlayerXPText;
    public TextMeshProUGUI PlayerLevelText;
    private int currentSelectShip;
    private LevelSystem levelSystem;

    private void Start()
    {
        PlayerData playerData = DataController.GetPlayerData();
        var Level = playerData.Level;
        var xp = playerData.xp;
        var xpToLevel = playerData.xpToLevel;
        levelSystem = new LevelSystem(Level, xp, xpToLevel);
        currentSelectShip = playerData.currentSelectedShip;

        SetLevelSystem(levelSystem);
    }

    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;
    }
    private void Update()
    {

        UpdateXPBarWidget();
    }
    public void UpdateXPBarWidget()
    {
        PlayerData playerData = DataController.GetPlayerData();
        LevelSystem levelSystem = PlayerManager.Instance.GetPlayerByID(playerData.currentSelectedShip).prefab.GetLevelSystem();

        if (levelSystem.GetLevel() >= levelSystem.GetMaxLevel())
        {
            PlayerLevelText.text = "" + levelSystem.GetLevel();
            PlayerXPText.text = "Maxed";
        }
        else
        {
            PlayerLevelText.text = "" + levelSystem.GetLevel();

            PlayerXPText.text = string.Format("{0}/{1}",
                levelSystem.GetXP(),
                Mathf.Round(levelSystem.GetXpToLevel()));
        }
    }
}