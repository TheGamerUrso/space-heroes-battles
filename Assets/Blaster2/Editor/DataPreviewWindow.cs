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


    [MenuItem("ProjectBlaster2/DataPreviewWindow")]
    public static void ShowWindow()
    {
        GetWindow<DataPreviewWindow>("Data Preview Window");
    }

    private void OnGUI()
    {
       
    }

}