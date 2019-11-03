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

    // Start is called before the first frame update
    private void Start()
    {
        int prevPoint = 0;
        for (int i = 0; i < Nodes.Length; i++)
        {
            if (i > 0)
            {
                prevPoint = i - 1;
            }
           // Debug.DrawLine(Nodes[prevPoint].position, Nodes[i].position, Color.red);
        }
    }

    private void Update()
    {
        int prevPoint = 0;
        for (int i = 0; i < Nodes.Length; i++)
        {
            if (i > 0)
            {
                prevPoint = i - 1;
            }
            //Debug.DrawLine(Nodes[prevPoint].position, Nodes[i].position, Color.red);
        }
    }
}