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
                    new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("name"), GUIContent.none);



                EditorGUI.PropertyField(
                    new Rect(rect.x + 65, rect.y, 100, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("poolGameObjectType"), GUIContent.none);



                EditorGUI.PropertyField(
                  new Rect(rect.x + 60 + 110, rect.y, rect.width - 60 - 120 - 30, EditorGUIUtility.singleLineHeight),
                  element.FindPropertyRelative("PoolElementGameObjects"), GUIContent.none);


                EditorGUI.PropertyField(
                  new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
                  element.FindPropertyRelative("poolIndex"), GUIContent.none);

            };

                serializedObject.Update();
                list.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
