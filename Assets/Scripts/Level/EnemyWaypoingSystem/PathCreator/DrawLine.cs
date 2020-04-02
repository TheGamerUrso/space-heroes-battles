using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DrawLine : MonoBehaviour
{
    public Transform[] Nodes;

    private void OnValidate()
    {
        var transforms = new HashSet<Transform>(GetComponentsInChildren<Transform>());
        transforms.Remove(transform);
        Nodes = transforms.ToArray();
    }

    private void OnDrawGizmosSelected()
    {
        int prevPoint = 0;
        for (int i = 0; i < Nodes.Length; i++)
        {
            if (i > 0)
            {
                prevPoint = i - 1;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Nodes[prevPoint].position, Nodes[i].position);
        }
    }
}