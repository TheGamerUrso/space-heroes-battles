using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using Malee.List;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class DataPreviewWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    public MonoScript mScript;

    PlayerData playerData;

    [MenuItem("ProjectBlaster2/DataPreviewWindow")]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        DataPreviewWindow window = (DataPreviewWindow)EditorWindow.GetWindow(typeof(DataPreviewWindow));
        window.minSize = new Vector2(1280, 720);
        window.Show();
    }
    void OnGUI()
    {
        EditorGUILayout.LabelField("PlayerData", EditorStyles.boldLabel);
        EditorGUIUtility.labelWidth = 75;

        PlayerData p = PersistantData.GetPlayerData();
        EditorGUIUtility.fieldWidth = (position.width) - 150;
    }

}