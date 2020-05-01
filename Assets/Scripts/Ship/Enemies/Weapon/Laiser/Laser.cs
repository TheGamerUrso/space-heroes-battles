using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public LaserSuper laserSuper;
    public bool active;
    public float maxLaserDistance;
    public LayerMask enemies;
    private RaycastHit hit;
    private float zHit;
    public bool fullCharge;
    public ParticleSystem hitEffect;
    public AudioSource sfx;

    public bool hitSomething;

    private void Start()
    {
        DeactiveLaser();
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
        if (laserSuper.SpecialActive)
        {
            m_LineRenderer.SetPosition(0, transform.position + new Vector3(0, 0, 5));

            zHit = 150;

        

            if (Physics.Raycast(transform.position, transform.forward, out hit, maxLaserDistance, enemies))
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
                            zHit = hit.point.z;       
                            damagable.TakeDamage(laserSuper.damage);
                        }
                        hitSomething = false; 
                    }
                }
                else
                {
                    hitSomething = false;
                }
            }

            m_LineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, zHit));
        }
    }
}
