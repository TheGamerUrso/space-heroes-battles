using TheGamerUrso;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    public Player Target;
    public Vector3 TargetLastPosition;
    private IDestroyable target;
    private GameObject explosion;

    private void FixedUpdate()
    {
        Movement();
    }

    public override void Movement()
    {
        if (FollowTarget)
        {
            // Aim bullet in player's direction.
            rigid.MovePosition(transform.position - TargetLastPosition * speed * Time.deltaTime);
            //  transform.position -= TargetLastPosition * speed * Time.deltaTime;
        }
        else
        {
            rigid.MovePosition(transform.position - transform.forward * speed * Time.deltaTime);
            // transform.position -= transform.forward * speed * Time.deltaTime;
        }

        Vector3 worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 ScreenToViewpoint = Camera.main.ScreenToViewportPoint(worldToScreen);

        if(transform.position.z < Constants.m_ZMin )
        {
            gameObject.SetActive(false);
        }

    }
    public override void DestoryNow()
    {
        explosion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.BulletExplosion);
        explosion.transform.SetPositionAndRotation(transform.position + Vector3.up * 2, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            IDestroyable destroyable = other.GetComponent<IDestroyable>();
            destroyable.TakeDamage(Damage);
            DestoryNow();
        }
    }

    public void GetTargetLastPosition()
    {
        Target = GameObject.FindObjectOfType<Player>();
        if (Target)
        {
            TargetLastPosition = (transform.position - Target.transform.position).normalized;
        }
    }

    public void SetFollowTarget(bool value)
    {
        FollowTarget = value;
    }
}