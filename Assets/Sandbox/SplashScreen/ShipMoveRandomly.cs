using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMoveRandomly : MonoBehaviour
{
    public float speed = 1.5f;
    public float rotateSpeed = 5.0f;

    Vector3 newPosition;

    void Start()
    {
        PositionChange();
    }

    void PositionChange()
    {
        newPosition = new Vector3(Random.Range(-2.0f, 2.0f), transform.localPosition.y, Random.Range(-1.0f, 1.0f));
    }

    void Update()
    {
        if (Vector3.Distance(transform.localPosition, newPosition) < 1)
            PositionChange();

        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * speed);

    }

}
