using UnityEngine;

public class Enemy4AI : SimpleAI
{
    private Vector3 destination;
    private float x_Vel;
    private float z_Vel;

    public float XSpeed;
    public float ZSpeed;

    public override void Move()
    {
        x_Vel = XSpeed;
        z_Vel = ZSpeed;

        if (transform.position.z < Constants.m_ZMin)
        {
            Destroy(gameObject);
            gameObject.SetActive(false);
        }

        var _newPos = transform.position;
        _newPos.x += x_Vel * Time.deltaTime;
        _newPos.z += z_Vel * Mathf.Sin(Time.time) * Time.deltaTime;
        transform.position = _newPos;
    }
}