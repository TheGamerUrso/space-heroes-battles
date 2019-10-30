using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartfieldController : MonoBehaviour
{
    public List<Stars> m_ListOfInstansiatedStars = new List<Stars>();

    public float m_TotalStars;
    public Stars m_StarPrefab;
    public float m_Speed;

   
    private void Start()
    {
        CreateStars();
    }


    private void CreateStars()
    {
        for (int i = 0; i < m_TotalStars; i++)
        {     
            Stars star = Instantiate(
                m_StarPrefab,
                Vector3.zero,
                Quaternion.identity) as Stars;
            m_ListOfInstansiatedStars.Add(star);
            star.transform.SetParent(transform);
        }

    }
}