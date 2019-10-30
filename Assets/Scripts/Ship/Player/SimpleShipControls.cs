using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleShipControls : MonoBehaviour
{
    public Rigidbody rigid;
    public GameObject FollowTarget;
    public RotateInput rotInput;
    public float rotSpeed;

    private Quaternion shipRotation;
    private Vector3 TargetRotation;

    public MouseInput mouseInput;
    private PlayerAnimation playerAnimation;
    private bool isMoving;
    public float speed;
    private float yMove = -50;
    private static float offspec = 10;
    private Vector3 targetPos;

    [SerializeField] private GameObject Ship;
    [SerializeField] private GameObject ShipModel;


    private bool Keyboard = true;

    private Touch currentTouch;
    private TouchPhase currentTouchPhase;
    private Quaternion targetRot;
    private Plane plane;
    private Ray ray;

    private float vertical;
    private float horizontal;


    private Vector3 dist;
    private Vector2 MaxScreenBound;
    private Vector2 MinScreenBound;

    public void Start()
    {
        FollowTarget = GameObject.Find("Pointer");
        targetPos = transform.position;
        rotInput = new RotateInput();
        UpdateOffset();
        OptionScreen.OnOptionsRecieved += UpdateOffset;
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
        plane = new Plane(Vector3.up, transform.position);
        rigid = GetComponent<Rigidbody>();
    }

    public static void UpdateOffset()
    {
        PlayerData playerData = DataController.GetPlayerData();
        offspec = playerData.distance;
    }

    public void Movement()
    {
        if (mouseInput.GetClickDown())
        {
            SetTarggetPosition();
            Rotate(); ;
        }
        else
        {
            Vector3 targetEulerAngels = ShipModel.transform.localEulerAngles;
            ShipModel.transform.localEulerAngles = new Vector3(targetEulerAngels.x
              , targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z, 0, .1f));
        }
    }

    private void MoveObject()
    {
        rigid.MovePosition(Vector3.MoveTowards(transform.position, targetPos + new Vector3(0, 0, offspec), speed * Time.deltaTime));

        if (transform.position == targetPos)
        {
            isMoving = false;
        }
    }

    private void Update()
    {
        Movement();
    }

    private void LateUpdate()
    {
        if (isMoving)
        {
            if (playerAnimation.GetAnimationState("Enter") == false && playerAnimation.GetAnimationState("Exit") == false)
            {
                MoveObject();
            }
        }
    }

    public void Rotate()
    {
        Vector3 targetEulerAngels = ShipModel.transform.localEulerAngles;
        ShipModel.transform.localEulerAngles = new Vector3(targetEulerAngels.x
          , targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z,-Input.GetAxis("Mouse X") * 15, .2f));

    }

    public void Rotate(Transform ship, Vector3 TargetRot)
    {
        var dist = (transform.position - targetPos).normalized;

        float turn = 25;

        if (dist.x > 0.5f)
        {
            turn = 40;
        }

        if (dist.x < -.5f)
        {
            turn = -40;
        }

        if (dist.x == 0)
        {
            turn = 0;
        }

        if (Keyboard)
        {
            ShipModel.transform.eulerAngles = new Vector3(ShipModel.transform.eulerAngles.x, ShipModel.transform.eulerAngles.y, -horizontal * 25);
        }
        else
        {
            ShipModel.transform.rotation = Quaternion.Lerp(ShipModel.transform.rotation, Quaternion.Euler(new Vector3(0, ShipModel.transform.eulerAngles.y, turn)), rotSpeed * Time.deltaTime);
        }

        shipRotation = ship.rotation;

        //targetRot = rotInput.GetRotateByCenter(transform, targetPos, TargetRot);
        // rigid.MoveRotation(Quaternion.Lerp(ship.rotation, targetRot, rotSpeed * Time.deltaTime));
    }

    private void SetTarggetPosition()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            float point;

            plane = new Plane(Vector3.up, transform.position);
            ray = Camera.main.ScreenPointToRay(FollowTarget.transform.position);
            point = 0f;

            if (plane.Raycast(ray, out point))
                targetPos = new Vector3(ray.GetPoint(point).x, yMove, ray.GetPoint(point).z) + new Vector3(0, 0, offspec);

            isMoving = true;
        }

    }
}