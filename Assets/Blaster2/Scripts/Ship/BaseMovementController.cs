using UnityEngine;

public class BaseMovementController : MonoBehaviour
{
    [SerializeField] protected float speed;
    public float Speed { get { return speed; } set { speed = value; } }
    public virtual void Move()
    {

    }

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }
}
