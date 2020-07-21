using Malee.List;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectControllerWindow : EditorWindow
{
    private string titleString = "Project Controls";
    private string scenePath = "Assets/Blaster2/_Scenes";

    [MenuItem("ProjectBlaster2/ProjectController")]
    public static void ShowWindow()
    {
        GetWindow<ProjectControllerWindow>("Project");
    }

    private void OnGUI()
    {  
        GUILayout.Label(titleString, EditorStyles.boldLabel);

        GUILayout.Label("Scene Manager");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Boot"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.boot.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.boot);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/"+LevelEnum.boot.ToString()+".unity");
            }
        }
        if (GUILayout.Button("Main"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Main.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Main);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/" + LevelEnum.Main.ToString() + ".unity");
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Level0"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level1.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level1);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level0.ToString() + ".unity");
            }
        }

        if (GUILayout.Button("Level1"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level1.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level1);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level1.ToString() + ".unity");
            }
        }
        if (GUILayout.Button("Level2"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level2.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level2);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level2.ToString() + ".unity");
            }
        }
        if (GUILayout.Button("Level3"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level3.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level3);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level3.ToString() + ".unity");
            }
        }
        if (GUILayout.Button("Level4"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level4.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level4);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level4.ToString() + ".unity");
            }
        }
        if (GUILayout.Button("Level5"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level5.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level5);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level5.ToString() + ".unity");
            }
        }
        if (GUILayout.Button("Level6"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level6.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level6);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level6.ToString() + ".unity");
            }
        }
        if (GUILayout.Button("Level7"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level7.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level7);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level7.ToString() + ".unity");
            }
        }
        if (GUILayout.Button("Level8"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level8.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level8);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level8.ToString() + ".unity");
            }
        }
        if (GUILayout.Button("Level9"))
        {
            if (EditorApplication.isPlaying)
            {
                if (!GameManager.Instance.currentLevelLoaded.Equals(LevelEnum.Level9.ToString()))
                {
                    GameManager.Instance.LoadScene(LevelEnum.Level9);
                }
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath + "/Levels/" + LevelEnum.Level9.ToString() + ".unity");
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("In Game");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Load"))
        {
            if (EditorApplication.isPlaying)
            {
                PersistantData.ReplacePlayerData(new PlayerData());
                PersistantData.Load();
            }
        }

        if (GUILayout.Button("Save"))
        {
            if (EditorApplication.isPlaying)
            {
                PersistantData.Save();
            }
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

        EditorGUILayout.EndHorizontal();

    }

    public void CompleteObjective(int index)
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        if ((ObjectiveType)playerData.ListOfOnGoingObjectives[index].objectiveType == ObjectiveType.Unharmed)
        {
            playerData.PlayedGame = true;
            playerData.GotHitInGame = false;
            playerData.ListOfOnGoingObjectives[index].UpdateProgress(1);
        }
        else { playerData.ListOfOnGoingObjectives[index].UpdateProgress(playerData.ListOfOnGoingObjectives[index].requirment); }

    }
}