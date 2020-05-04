using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public float verticalSize;
    public float speed;
    public float MovementDelay;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        verticalSize = boxCollider.bounds.size.z + 50;
    }

    private void LateUpdate()
    {
        transform.Translate(-transform.forward * speed * Time.deltaTime);
        //transform.Translate(-transform.forward * speed * Time.deltaTime);
        // if (OutOfRange())
        // {
        //     transform.position = transform.position + new Vector3(0, 0, (verticalSize - 50) * 3);
        // }

        //transform.Translate(-transform.forward * speed * Time.deltaTime);

        if (OutOfRange())
        {
            transform.position += new Vector3(0, 0, (verticalSize - 50) * 3);
        }
    }

    public bool OutOfRange()
    {
        if (transform.position.z < -verticalSize)
        {
            return true;
        }
        return false;
    }
}