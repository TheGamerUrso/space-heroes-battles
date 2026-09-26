using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserWeapon : MonoBehaviour
{   
    public Enemy user;
    public LineRenderer m_LineRenderer;
    public bool active;
    public float maxLaserDistance;
    public LayerMask enemies;
    private RaycastHit hit;
    private float xHit;
    public bool fullCharge;
    public ParticleSystem hitEffect;
    public AudioSource sfx;

    public bool hitSomething;
    public int direciton;
    public float laserSize;
    private void Start()
    {
        ActiveLaser();
    }

    public void DeactiveLaser()
    {
        active = false;
        hitEffect.gameObject.SetActive(false);
        sfx.Stop();
    }

    public void ActiveLaser()
    {
        active = true;
        hitEffect.gameObject.SetActive(true);
        sfx.Play();
    }

    private void Update()
    {
        direciton = user.GetComponent<LaserEnemyMovement>().left ? 1 : -1;

         if (active)
        {
            laserSize = Mathf.Lerp(laserSize, 4, 1);

            if (laserSize > 4)
            {
                laserSize = 4;
            }

            m_LineRenderer.widthMultiplier = laserSize;
        }
        if (laserSize == 4)
        {
            fullCharge = true;
        }

        if (hitSomething)
        {
            hitEffect.Play();
            hitEffect.gameObject.SetActive(true);
        }
        else
        {
            hitEffect.gameObject.SetActive(false);
        }
    }
    
    public void LateUpdate()
    {
        if (active)
        {
            var calculatedDirection = 3 * direciton;
            m_LineRenderer.SetPosition(0, transform.position + new Vector3(calculatedDirection, 0, 0));

            xHit = 150;

            if (Physics.Raycast(transform.position,transform.forward, out hit, maxLaserDistance, enemies))
            {
                if (hit.collider != null)
                {
                    Debug.Log("hit something");

                    if (fullCharge)
                    {
                        IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                        if (damagable != null)
                        {
                            hitSomething = true;
                            hitEffect.transform.position = hit.point;
                            xHit = hit.point.x;       
                            damagable.TakeDamage(user.Damage);
                        }
                        hitSomething = false; 
                    }
                }
                else
                {
                    hitSomething = false;
                }
            }

            m_LineRenderer.SetPosition(1, new Vector3(xHit * calculatedDirection,transform.position.y, transform.position.z));
        }
    }
}
