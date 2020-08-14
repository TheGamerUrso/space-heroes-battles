using Malee.List;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private SerializedProperty list;
    private ReorderableList relist;

    private SerializedProperty MusicixerGroup;
    private SerializedProperty SFXMixerGroup;

    private void OnEnable()
    {
        list = serializedObject.FindProperty("MusicClips");
        relist = new ReorderableList(list);

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

     
        serializedObject.Update();
        relist.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(1f);
        base.OnInspectorGUI();

    }
}
