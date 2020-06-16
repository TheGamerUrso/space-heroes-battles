using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private ReorderableList list;

    private SerializedProperty MusicixerGroup;
    private SerializedProperty SFXMixerGroup;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("MusicClips"),
                true, true, true, true);

        MusicixerGroup = serializedObject.FindProperty("MusicMixerGroup");
        SFXMixerGroup = serializedObject.FindProperty("SFXMixerGroup");
    }
    public override void OnInspectorGUI()
    {
      
        AudioManager audioManager = (AudioManager)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Previous", GUILayout.Width(80), GUILayout.Height(40)))
        {
            audioManager.PlayPrevious();
        }

        if (GUILayout.Button("Stop", GUILayout.Width(80), GUILayout.Height(40)))
        {
            audioManager.StopMusic();
        }

        if (GUILayout.Button("Play",GUILayout.Width(80), GUILayout.Height(40)))
        {
            audioManager.PlayRandomSong();
            audioManager.PlayMusic(audioManager.MusicClips[Random.Range(0, audioManager.MusicClips.Count)].audioClip);
        }

        if (GUILayout.Button("Next", GUILayout.Width(80), GUILayout.Height(40)))
        {
            audioManager.PlayNext();
        }

        GUILayout.EndHorizontal();

        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Playlist");
        };

        list.drawElementCallback =
    (Rect rect, int index, bool isActive, bool isFocused) => {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;


        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("Name"), GUIContent.none);


        EditorGUI.PropertyField(
            new Rect(rect.x + 60, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("audioClip"), GUIContent.none);
    };

        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(1f);
        base.OnInspectorGUI();

    }
}
