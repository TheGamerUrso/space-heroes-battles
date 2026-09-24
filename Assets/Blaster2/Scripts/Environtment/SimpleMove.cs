using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.position -= transform.forward * speed * Time.deltaTime;
        if (transform.position.z < Constants.m_ZMin)
        {
            gameObject.SetActive(false);
        }
    }
}