using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleShipControls : MonoBehaviour
{
    public Rigidbody rigid;
    public MouseInput mouseInput;
    public RotateInput rotInput;
    public float rotSpeed;

    private PlayerShip playerShip;


    private bool isMoving;
    private float speed;

    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }

    private float yMove = -50;
    private static float offspec = 10;
    public Vector2 touchPosOffset;
    private Vector3 targetPos;

    [SerializeField] private GameObject ShipModel;

    private Plane plane;
    private Ray ray;


    private bool blockMovement;
    public void Start()
    {
        targetPos = transform.position;
        rotInput = new RotateInput();

        PlayerData playerData = DataController.GetPlayerData();
        playerData.distanceChanged = UpdateOffset;
        offspec = playerData.distance;

        plane = new Plane(Vector3.up, transform.position);
        rigid = GetComponent<Rigidbody>();

        playerShip = GetComponent<PlayerShip>();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public static void UpdateOffset(float ammount)
    {
        offspec = ammount;
    }

    public void SetTargetPosition()
    {
        float point;
        plane = new Plane(Vector3.up, transform.position);
        ray = Camera.main.ScreenPointToRay(mouseInput.GetTouchPosition());
        point = 0f;
        offspec = 0f;
        if (plane.Raycast(ray, out point))
            targetPos = new Vector3(ray.GetPoint(point).x, yMove, ray.GetPoint(point).z) + new Vector3(0, 0, offspec);
    }

    public void GetPlayerInput()
    {
        if (mouseInput.GetClickDown())
        {
            SetTargetPosition();
            isMoving = true;

            Rotate();
   
        }
        else
        {
            Vector3 targetEulerAngels = ShipModel.transform.localEulerAngles;
            ShipModel.transform.localEulerAngles = new Vector3(targetEulerAngels.x
              , targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z, 0, .1f));
            
        }



    }

    public bool IsEnterOrExitAnimationState()
    {
        return playerShip.GetAnimationState("Enter") == false && playerShip.GetAnimationState("Exit") == false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, targetPos + new Vector3(0, 0, offspec));
    }
    private void Move()
    {
        if (GameSession.IsGameOver)
        {
            return;
        }

        if (isMoving)
        {
            if (IsEnterOrExitAnimationState())
            {
                Vector3 direction = targetPos + new Vector3(0, 0, offspec) - transform.position;

                if (direction.magnitude > 1) {

                    Debug.DrawRay(this.transform.position, direction, Color.red);
                    transform.Translate(direction.normalized * speed * Time.deltaTime,Space.World);
                }

            }
        }
    }

    private void Update()
    {
        GetPlayerInput();
 

    }

    private void LateUpdate()
    {
        Move();
    }

    public void Rotate()
    {
        Vector3 targetEulerAngels = ShipModel.transform.localEulerAngles;
        ShipModel.transform.localEulerAngles = new Vector3(targetEulerAngels.x
          , targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z, -Input.GetAxis("Mouse X") * 15, .2f));

    }

}