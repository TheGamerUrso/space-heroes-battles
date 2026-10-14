using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(DebugManager))]
public class DebugManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields first
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Actions", EditorStyles.boldLabel);

        DebugManager debugManager = (DebugManager)target;
        System.Type type = debugManager.GetType();

        // Get all methods in the DebugManager class
        MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (MethodInfo method in methods)
        {
            // Check if the method has our [Button] attribute
            ButtonAttribute buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
            if (buttonAttr != null)
            {
                // Use the custom name provided in the attribute, or fallback to the method name
                string label = string.IsNullOrEmpty(buttonAttr.ButtonName) ? method.Name : buttonAttr.ButtonName;

                if (GUILayout.Button(label))
                {
                    // Invoke the method when the button is clicked in the Inspector
                    method.Invoke(debugManager, null);
                }
            }
        }
    }
}