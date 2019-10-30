using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laizer : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public float m_Distance;
    RaycastHit hit;
    public GameObject m_HitEffect;

    void Start()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    

        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, transform.position + transform.forward * m_Distance);

        m_HitEffect.SetActive(false);
        m_Distance = 256;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            m_HitEffect.transform.position = hit.point;
        m_Distance = Vector3.Distance(transform.position, hit.point);
            m_HitEffect.SetActive(true);
        }
    }
}
