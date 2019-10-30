using System.Collections;
using UnityEngine;

public class Stars : MonoBehaviour
{
    public float speed;

    private Vector2 m_PointsOnX;
    private Vector2 m_PointsOnY;
    private Vector2 m_PointsOnZ;

    private void Start()
    {
        m_PointsOnX = new Vector2(Constants.m_XMin, Constants.m_XMax);
        m_PointsOnY = new Vector2(-100, 100);
        m_PointsOnZ = new Vector2(-Constants.m_ZMin, Constants.m_ZMax);

        int numX = (int)UnityEngine.Random.Range(m_PointsOnX.x, m_PointsOnX.y);
        int NumY = (int)UnityEngine.Random.Range(m_PointsOnY.x, m_PointsOnY.y);
        int NumZ = (int)UnityEngine.Random.Range(m_PointsOnZ.x, m_PointsOnZ.y);

        transform.localPosition = new Vector3(numX, NumY, NumZ);
    }

    private void Update()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (transform.localPosition.z < m_PointsOnZ.x-50)
        {
            Reset(m_PointsOnX, m_PointsOnY, m_PointsOnZ);
        }
    }

    public void Move()
    {
        transform.position -= transform.forward * speed * Time.deltaTime;
 
    }

    public void Reset(Vector2 PointsOnX, Vector2 PointsOnY, Vector2 PointsOnZ)
    {
        int numX = (int)UnityEngine.Random.Range(PointsOnX.x, PointsOnX.y);
        int NumY = (int)UnityEngine.Random.Range(PointsOnY.x, PointsOnY.y);
        int NumZ = (int)PointsOnZ.y;

        transform.localPosition = new Vector3(numX, NumY, NumZ);

    }
}