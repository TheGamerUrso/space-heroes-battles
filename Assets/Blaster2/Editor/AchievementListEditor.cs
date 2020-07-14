using Malee.List;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AchievementList))]
[CanEditMultipleObjects]
public class AchievementListEditor : Editor
{
    AchievementList achievelemtList;
    SerializedProperty list;
    ReorderableList reList;

    private void OnEnable()
    {
        achievelemtList = target as AchievementList;
        list = serializedObject.FindProperty("ListOfAchievelemtnts");
        reList = new ReorderableList(list);

        reList.onAddCallback += delegate { OnAdd(); };
        reList.onReorderCallback += delegate { OnReorder(); };
        reList.onRemoveCallback += delegate { OnRemove(reList.Selected); };
    }

    private void OnAdd()
    {
        Achievement achievement = new Achievement()
        {
            ID = achievelemtList.ListOfAchievelemtnts.Count
        };
        achievelemtList.ListOfAchievelemtnts.Add(achievement);
        achievelemtList.Reorder();
    }

    private void OnReorder()
    {
        achievelemtList.Reorder();
    }

    private void OnRemove(int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            achievelemtList.ListOfAchievelemtnts.RemoveAt(i);
        }
        achievelemtList.Reorder();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        reList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
