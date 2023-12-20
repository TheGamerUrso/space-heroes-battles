using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMove : MonoBehaviour
{
    private int Direction;
    private bool directionChanged;
    private Vector3 movement;
    private float maneuverTimer = 2;
    [SerializeField] private float Speed;
    public Transform AsteroidTransform;
    public float rotSpeed;

    private void OnEnable()
    {
        Maneuver();

        rotSpeed = Random.Range(10, 20);
    }

    public void LateUpdate()
    {
        movement = -transform.forward * Speed;
        movement.x *= Direction * (Speed / 2);

        transform.position += movement * Time.deltaTime;

        AsteroidTransform.Rotate(Vector3.right, rotSpeed * Time.deltaTime);

        if (transform.position.x > Constants.m_XMax)
        {
            Direction = 1;
        }
        else if (transform.position.x < Constants.m_XMin)
        {
            Direction = -1;
        }

        if (transform.position.z < Constants.m_ZMin || transform.position.z > 140 || transform.position.y > 140 || transform.position.y < -127)
        {
            gameObject.SetActive(false);
        }
    }

    private void Maneuver()
    {
        int random = UnityEngine.Random.Range(0, 100);

        if (random <= 33.33)
        {
            Direction = 1;
        }
        else if (random > 33.33 && random <= 66.66)
        {
            Direction = -1;
        }
        else
        {
            Direction = 0;
        }

        Direction = 0;
    }
}
