using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ProjectControllerWindow : EditorWindow
{
    //private SpawnEnemies spawnEnemies;
    private PlayerShip player;

    private string CoinToEarnText;
    private int coinsToEarn;

    private string titleString = "Project Controls";
    private string scenePath = "Assets/Scenes";
    private string TestModeStrig = "TestMode";
    private int CompleteLevelIndex;
    private string BaseXpToEarn = "";
    private float xpToEarn;

    public PlayerShipElement currentPlayer;

    public Vector3 PosInWorld;
    public Vector3 resets;
    public float m_DistanceZ;

    [MenuItem("ProjectBlaster2/ProjectController")]
    public static void ShowWindow()
    {
        GetWindow<ProjectControllerWindow>("Project");
    }

    private void OnGUI()
    {
        //WindowCode

        GUILayout.Label(titleString, EditorStyles.boldLabel);

        GUILayout.Label("Scene Manager");

        if (GUILayout.Button("Main"))
        {
            EditorSceneManager.OpenScene(scenePath + "/Main.unity");
        }

        GUILayout.Label("In Game");

        EditorGUILayout.BeginHorizontal();

        //if (GUILayout.Button("Load Boss"))
        //{
        //    if (EditorApplication.isPlaying)
        //    {
        //        if (spawnEnemies == null)
        //        {
        //            GameObject go = GameObject.Find("Spawning");
        //            spawnEnemies = go.GetComponent<SpawnEnemies>();
        //        }

        //        spawnEnemies.MaxWave = 2;
        //    }
        //}
        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("Load"))
        {
            if (EditorApplication.isPlaying)
            {
                DataController.SetPlayerData(new PlayerData());
                SaveSystem.LoadGame();
            }
        }

        if (GUILayout.Button("Save"))
        {
            if (EditorApplication.isPlaying)
            {
                SaveSystem.SaveGame();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        BaseXpToEarn = EditorGUILayout.TextField(BaseXpToEarn);
        if (string.IsNullOrEmpty(BaseXpToEarn) == false)
            xpToEarn = float.Parse(BaseXpToEarn);

        if (GUILayout.Button("Earn XP"))
        {
            if (EditorApplication.isPlaying)
            {
                PlayerData playerData = DataController.GetPlayerData();
                playerData.EarnXP(xpToEarn);
            }
        }
        EditorGUILayout.EndVertical();



        if (GUILayout.Button("Toggle Invisibility"))
        {
            if (EditorApplication.isPlaying)
            {
                if (player == null)
                {
                    player = GameObject.FindObjectOfType<PlayerShip>();
                }

                player.GetComponent<BoxCollider>().enabled = !player.GetComponent<BoxCollider>().enabled;
            }
        }

        EditorGUILayout.BeginVertical();

        CoinToEarnText = EditorGUILayout.TextField("");
        if (string.IsNullOrEmpty(CoinToEarnText) == false)
            coinsToEarn = int.Parse(CoinToEarnText);

        if (GUILayout.Button("Earn Coins"))
        {
            if (EditorApplication.isPlaying)
            {
                PlayerData playerData = DataController.GetPlayerData();
                playerData.Coins += coinsToEarn;
            }
        }
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("CompleteGame"))
        {
            if (EditorApplication.isPlaying)
            {
                GameController.OnWin?.Invoke(GameController.Instance);
            }
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Objective Debug Menu");
        if (GUILayout.Button("Complete First Objective"))
        {
            if (EditorApplication.isPlaying)
            {
                CompleteObjective(0);
            }
        }

        if (GUILayout.Button("Complete First Objective"))
        {
            if (EditorApplication.isPlaying)
            {
                CompleteObjective(1);
            }
        }

        if (GUILayout.Button("Complete First Objective"))
        {
            if (EditorApplication.isPlaying)
            {
                CompleteObjective(2);
            }
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Level Quests Debug Menu");

        CompleteLevelIndex = EditorGUILayout.IntField(CompleteLevelIndex);

        if (GUILayout.Button("Complete First Challenge"))
        {
            PlayerData playerData = DataController.GetPlayerData();
            MissionCollection missionCollection = DataController.GetMissionCollection();
            Mission mission = missionCollection.GetMission(playerData.LevelUnlocked++);

            LevelObjectiveData[] levelObjectiveDatas = playerData.GetLevelObjectives("Level" + mission.ID);
            if (EditorApplication.isPlaying)
            {
                for (int i = 0; i < levelObjectiveDatas.Length; i++)
                {
                    levelObjectiveDatas[i].completed = true;
                }
            }           

            BriefingScreen briefingScreen = GameObject.FindObjectOfType<BriefingScreen>();
   

            playerData.LevelUnlocked = mission.ID+1;

            briefingScreen.RefreshLevelElements();

            //GameObject.FindObjectOfType<BriefingScreen>().LevelElements[CompleteLevelIndex].GetComponent<LevelElement>().Refresh();

        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Game Data");
        
    
        if (GUILayout.Button("Delete"))
        {
            PlayerPrefs.DeleteAll();
            SaveSystem.Delete();

        }



        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

    }

    public void CompleteObjective(int index)
    {
        PlayerData playerData = DataController.GetPlayerData();
        if ((ObjectiveType)playerData.ListOfOnGoingObjectives[index].objectiveType == ObjectiveType.Unharmed)
        {
            playerData.PlayedGame = true;
            playerData.GotHitInGame = false;
            playerData.ListOfOnGoingObjectives[index].UpdateProgress(1);
        }
        else { playerData.ListOfOnGoingObjectives[index].UpdateProgress(playerData.ListOfOnGoingObjectives[index].requirment); }

    }
}