using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : EnemyProjectile
{
    [SerializeField] private GameObject m_Prefab;
    public bool m_FollowPlayer;
    private float Angle = 22.5f;

    //====================================================================================================
    void Start()
    {
        StartCoroutine("ShootAllDirection");
    }

    //====================================================================================================
    IEnumerator ShootAllDirection()
    {
        int num = 0;
        GameObject newBullet;
        List<GameObject> listOfBullets = new List<GameObject>();
        do
        {
            newBullet = Instantiate(m_Prefab, transform.position,Quaternion.Euler(new Vector3(0, Angle * num, 0))) as GameObject;
            newBullet.SetActive(false);
            newBullet.GetComponent<Projectile>().Damage = damage;
            listOfBullets.Add(newBullet);
            newBullet.name = "#" + Angle * num;
      
            num++;
            yield return null;
        } while (num <= 15);
        foreach (GameObject item in listOfBullets)
        {
            item.SetActive(true);
            
        }
        Destroy(gameObject);
    }


}
