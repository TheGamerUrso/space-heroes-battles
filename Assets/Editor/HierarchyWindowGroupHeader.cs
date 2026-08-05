
using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad]
public static class HierarchyWindowGroupHeader
{
    static HierarchyWindowGroupHeader()
    {
        EditorApplication.hierarchyWindowItemByEntityIdOnGUI += HierarchyWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI(EntityId entityId, Rect selectionRect)
    {
        var gameObject = EditorUtility.EntityIdToObject(entityId) as GameObject;

        if (gameObject != null && gameObject.name.StartsWith("---", System.StringComparison.Ordinal))
        {
            EditorGUI.DrawRect(selectionRect, Color.gray);
            EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace("-", "").ToUpperInvariant());
        }
    }
}