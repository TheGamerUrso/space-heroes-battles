using Malee.List;
using UnityEditor;
using UnityEngine;

namespace TheGamerUrso
{
    namespace PoolSystem
    {
        [CustomEditor(typeof(PoolManager))]
        [CanEditMultipleObjects]
        public class PoolSystemEditor : Editor
        {
            private PoolManager poolManager;
            private SerializedProperty list;
            private ReorderableList relist;

            private void OnEnable()
            {
                poolManager = target as PoolManager;
                list = serializedObject.FindProperty("PoolElements");
                relist = new ReorderableList(list);
                relist.onAddCallback += delegate { OnAdd(); };
                relist.onRemoveCallback += delegate { OnRemove(relist.Selected); };
                relist.onReorderCallback += delegate { OnReorder(); };
            }

            public void OnAdd()
            {
                PoolElement poolElement = new PoolElement()
                {
                   
                };

                poolManager.PoolElements.Add(poolElement);
                poolManager.Recheck();
            }

            public void OnRemove(int[] ids)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    poolManager.PoolElements.RemoveAt(ids[i]);
                }
              
            }

            public void OnReorder()
            {
                poolManager.Recheck();
            }

            public override void OnInspectorGUI()
            {
                PoolManager poolingSystem = (PoolManager)target;     
                serializedObject.Update();
                relist.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
