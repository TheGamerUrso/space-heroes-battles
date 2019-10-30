using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    public GameObject[] Clouds;
    public List<GameObject> ListOfClouds = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < Clouds.Length; i++)
        {
            GameObject cloud = Instantiate(Clouds[i]);
            cloud.SetActive(false);
            cloud.transform.SetParent(transform);
            ListOfClouds.Add(cloud);
        }

        InvokeRepeating("GenerateCloud", 2, 4);
    }

    public void GenerateCloud()
    {
        Vector3 randoLoc = new Vector3(Random.Range(-30, 35), transform.position.y, Random.Range(150, 180));
        List<GameObject> UnusedCloud = new List<GameObject>();

        ListOfClouds.ForEach((c) =>
        {
            if (c.activeSelf == false)
            {
                UnusedCloud.Add(c);
            }
        });
        if (UnusedCloud.Count > 0)
        {
            GameObject cloud = UnusedCloud[Random.Range(0, UnusedCloud.Count)];
            cloud.transform.position = randoLoc;
            cloud.transform.rotation = Quaternion.identity;
            cloud.SetActive(true);
        }
    }
}