using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public bool Random;
    public Vector3 m_Rotation;
    public bool useDeltaTime;

    private void Start()
    {
        if (Random)
        {
            m_Rotation = new Vector3(0, 0, UnityEngine.Random.Range(-50, 50));
        }
    }

    private void Update()
    {
        if (useDeltaTime)
        {
            transform.Rotate(new Vector3(m_Rotation.x, m_Rotation.y, m_Rotation.z) * Time.deltaTime);
        }
        else
        {
            transform.Rotate(new Vector3(m_Rotation.x, m_Rotation.y, m_Rotation.z));
        }
    }
}