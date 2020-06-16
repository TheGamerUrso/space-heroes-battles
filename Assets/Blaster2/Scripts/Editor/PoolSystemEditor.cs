using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace TheGamerUrso
{
    namespace PoolSystem
    {
        [CustomEditor(typeof(PoolManager))]
        [CanEditMultipleObjects]
        public class PoolSystemEditor : Editor
        {
            private ReorderableList list;
            private void OnEnable()
            {
                list = new ReorderableList(serializedObject,
                        serializedObject.FindProperty("PoolElements"),
                        true, true, true, true);


            }

            public override void OnInspectorGUI()
            {
                PoolManager poolingSystem = (PoolManager)target;

                list.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Pooling System");
                };

                list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {

                var element = list.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 2;


                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, 128, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("name"), GUIContent.none);



                EditorGUI.PropertyField(
                    new Rect(rect.x + 128, rect.y, 128, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("poolGameObjectType"), GUIContent.none);



                EditorGUI.PropertyField(
                  new Rect(rect.x + 128 + 128, rect.y, 200, EditorGUIUtility.singleLineHeight),
                  element.FindPropertyRelative("PoolElementPrefab"), GUIContent.none);


                EditorGUI.PropertyField(
                  new Rect(rect.x + 128+ 128+ 200, rect.y, 128, EditorGUIUtility.singleLineHeight),
                  element.FindPropertyRelative("poolIndex"), GUIContent.none);

            };

                serializedObject.Update();
                list.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
