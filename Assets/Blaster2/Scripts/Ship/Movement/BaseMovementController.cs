using UnityEngine;

public class BaseMovementController : MonoBehaviour
{
    public Ship ship;
    [SerializeField] protected float speed;
    public float Speed { get { return speed; } set { speed = value; } }

    public void Setup(Ship ship)
    {
        this.ship = ship;
        UpdateSpeed();
    }

    public virtual void Move()
    {

    }

    public void UpdateSpeed()
    {
        Speed = ship.shipData.Speed;
    }
}
