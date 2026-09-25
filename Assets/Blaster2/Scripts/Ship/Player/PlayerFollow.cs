using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed;

    void Update()
    {
        if (target == null)
        {
            return;
        }
        else if (target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            Vector3 normalizedDir = dir.normalized;


            transform.position = Vector3.Lerp(transform.position, target.transform.position, speed * Time.deltaTime);


            transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -12, 12), transform.position.y, 0);
        }
    }
}
