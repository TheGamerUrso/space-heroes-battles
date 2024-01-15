using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLimitGridIndicator : MonoBehaviour
{
    public Transform ship;
    public GameObject[] limit;
    public LayerMask layerMask;
    public float distance = 5;
 public float maxDistance = 5;
    void Update()
    {
        if (transform.position.z > 25)
        {
            distance = transform.position.z / 3;
        }
        else
        {
            distance = maxDistance;
        }

        distance = Mathf.Clamp(distance, maxDistance, 100);

        bool leftLimit = Physics.Raycast(transform.position, transform.position + Vector3.left, distance, layerMask);
        bool righttLimit = Physics.Raycast(transform.position, transform.position + Vector3.right, distance, layerMask);

        Debug.DrawLine(transform.position, transform.position + Vector3.right * distance, Color.yellow, 1);
        Debug.DrawLine(transform.position, transform.position + Vector3.left * distance, Color.green, 1);


        limit[0].SetActive(leftLimit);
        limit[1].SetActive(righttLimit);
    }
}
