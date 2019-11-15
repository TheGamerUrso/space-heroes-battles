using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
public class DataPreviewWindow : EditorWindow
{
    Vector2 scrollPos;
    Vector2 LevelChallengesScrollPos;
    //private SpawnEnemies spawnEnemies;
    private Player player;

    private string CoinToEarnText;
    private int coinsToEarn;

    //private string titleString = "Project Controls";
    //private string scenePath = "Assets/Scenes";
   // private string TestModeStrig = "TestMode";

    //private string BaseXpToEarn = "";
    //private float xpToEarn;

    public Player currentPlayer;

    public Vector3 PosInWorld;
    public Vector3 resets;
    public float m_DistanceZ;

    [MenuItem("ProjectBlaster2/DataPreviewWindow")]
    public static void ShowWindow()
    {
        GetWindow<DataPreviewWindow>("Data Preview Window");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        //WindowCode

        //if (PlayerManager.instance)
        //    currentPlayer = PlayerManager.instance.GetPlayer();
        PlayerData playerData = null;

        if (DataController.Instance != null)
        {
            playerData = DataController.GetPlayerData();
        }

        if (Application.isPlaying && playerData != null)
        {
       

            string PlayerDataText =
                "Level : " + playerData.Level + "\n" +
                "HighScore : " + playerData.HighScore + "\n" +
                "GotHitInGame : " + playerData.GotHitInGame +
                "AutoAttack : " + playerData.AutoAttack + "\n" +
                "Coins : " + playerData.Coins + "\n" +
            "currentSelectedShip : " + playerData.currentSelectedShip + "\n" +
                "distance : " + playerData.distance + "\n" +
                "LevelUnlocked : " + playerData.LevelUnlocked + "\n";
            GUIContent guiContent = new GUIContent(PlayerDataText);

            GUILayout.Box(guiContent);
            GUILayout.BeginHorizontal();
            if (playerData.Upgrades.Length > 0)
            {
                string[] values = Enum.GetNames(typeof(UpgradeType));

                for (int i = 0; i < playerData.Upgrades.Length; i++)
                {
                    GUILayout.Label(values[i], EditorStyles.boldLabel,GUILayout.Width(64), GUILayout.Height(64));
                    GUILayout.Box(""+playerData.Upgrades[i],GUILayout.Width(32),GUILayout.Height(32));
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(2);
            LevelChallengesScrollPos = EditorGUILayout.BeginScrollView(LevelChallengesScrollPos,GUILayout.Height(150));
            GUILayout.Label("Objective Data", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            foreach (var item in playerData.GetListOfObjectives())
            {
                GUILayout.BeginVertical();
                GUILayout.Label(item.Key, EditorStyles.boldLabel);


                for (int i = 0; i < item.Value.Length; i++)
                {
                    string ObjectiveString = "" + item.Value[i].ID + ":" + "Completed : " + item.Value[i].completed;
                    GUILayout.Box(ObjectiveString);
                    GUILayout.Space(1);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            GUILayout.Label("Objective Data", EditorStyles.boldLabel);

            string objectivesString = "";
            for (int i = 0; i < playerData.ListOfOnGoingObjectives.Count; i++)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(""+ playerData.ListOfOnGoingObjectives[i].Id, EditorStyles.boldLabel);
                objectivesString = ""+ playerData.ListOfOnGoingObjectives[i].progress + ":" + playerData.ListOfOnGoingObjectives[i].requirment ;
                GUILayout.Box(objectivesString);
                GUILayout.EndVertical();
            }

        }
        else
        {
            GUILayout.Label("Run Game");
        }
        EditorGUILayout.EndScrollView();
    }

}