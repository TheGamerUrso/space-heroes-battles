using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleShipControls : MonoBehaviour
{
    private bool blockMovement;

    public float tilt;
    public float Speed;

    public Vector2 touchPosOffset;
    private Vector3 targetPos;

    [SerializeField] private GameObject ShipModel;

    private Plane plane;
    private Ray ray;
    public Vector3 offspec;

    private Touch currentTouch;
    private Vector2 currentTouchPos;

    private float yMove = -50;

    public float movementSensitivity = .1f;
    public float sensitivityScale = .1f;

    private Vector3 direction;


    public void Start()
    {
        targetPos = transform.position;

        PlayerData playerData = GameManager.Instance.GetPlayerData();

        playerData.distanceChanged = UpdateOffset;

        offspec = new Vector3(0, 0, playerData.Distance);

        plane = new Plane(Vector3.up, transform.position);

    }

    public void UpdateOffset(float ammount)
    {
        offspec = new Vector3(0, 0, ammount);
    }

    public void SetTargetPosition()
    {

        if (Input.touchCount == 0)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                currentTouch = Input.GetTouch(0);
                switch (currentTouch.phase)
                {
                    case TouchPhase.Began:
                        currentTouchPos = currentTouch.position;
                        break;
                    case TouchPhase.Moved:
                        currentTouchPos = currentTouch.position;
                        ray = Camera.main.ScreenPointToRay(currentTouchPos);
                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                        currentTouchPos = transform.position;
                        break;
                }
            }

        }


        float point;
        plane = new Plane(Vector3.up, transform.position);
        point = 0f;
        if (plane.Raycast(ray, out point))
            targetPos = new Vector3(ray.GetPoint(point).x, yMove, ray.GetPoint(point).z);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, targetPos + offspec);
    }


    private void Move()
    {
        //Vector3 initPos = Vector3.zero;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    initPos = transform.position;
        //}

        //Cursor.lockState = CursorLockMode.Locked;

        //float xVel = Input.GetAxisRaw("Mouse X") * Speed * movementSensitivity * Time.deltaTime;
        //float zVel = Input.GetAxisRaw("Mouse Y") * Speed * movementSensitivity * Time.deltaTime;

        //transform.Translate(new Vector3(xVel, 0, zVel)  , Space.World);

        if (direction.magnitude > .1f)
        {
            transform.Translate((direction + offspec) * Speed * movementSensitivity * Time.deltaTime, Space.World);
        }


    }

    private void Update()
    {
        direction = targetPos - transform.position;

        if (direction.magnitude >= .1f)
        {
            movementSensitivity += Time.deltaTime;
        }
        else if (direction.magnitude < .1f)
        {
            movementSensitivity -= Time.deltaTime;
        }

        movementSensitivity = Mathf.Clamp(movementSensitivity, 0, 1f);

        if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && !IsMouseOverUI())
        {
            SetTargetPosition();
            Rotate();
        }
    }
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void LateUpdate()
    {
        Move();
    }

    public void Rotate()
    {
        var rotVelocity = -(Input.GetAxis("Mouse X")) * tilt;

        if (rotVelocity > .1f || rotVelocity < -.1f && (Input.touchCount > 0 || Input.GetMouseButton(0)))
        {
            Vector3 targetEulerAngels = ShipModel.transform.localEulerAngles;
            ShipModel.transform.localEulerAngles = new Vector3(
                targetEulerAngels.x
              , targetEulerAngels.y,
                Mathf.LerpAngle(
                  targetEulerAngels.z,
             rotVelocity, .1f));
        }
        else
        {
            Vector3 targetEulerAngels = ShipModel.transform.localEulerAngles;
            ShipModel.transform.localEulerAngles =
                new Vector3(
                    targetEulerAngels.x
              , targetEulerAngels.y,
                    Mathf.LerpAngle(targetEulerAngels.z, 0, .1f));
        }
    }

    public bool CheckIfTouchIsOverUI(Touch touch)
    {
        int id = touch.fingerId;
        if (EventSystem.current.IsPointerOverGameObject(id))
        {
            return true;
        }
        return false;
    }

    /**
     *  Version 1
    

    public Rigidbody rigid;
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


    private const String HorizontalMouse = "HorizontalMouse";
    private const int MOUSE = 0;
    private Touch currentTouch;
    private TouchPhase currentTouchPhase;
    private Vector2 previousTouchPos;

    private bool blockMovement;

    public void Start()
    {
        targetPos = transform.position;

        PlayerData playerData = GameManager.Instance.GetPlayerData();
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
        ray = Camera.main.ScreenPointToRay(GetTouchPosition());
        point = 0f;
        if (plane.Raycast(ray, out point))
            targetPos = new Vector3(ray.GetPoint(point).x, yMove, ray.GetPoint(point).z) + new Vector3(0, 0, offspec);
    }

    public void GetPlayerInput()
    {
        if (GetClickDown())
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

    public Vector3 GetMousePos()
    {
        return Input.mousePosition;
    }
    public bool CheckIfTouchIsOverUI(Touch touch)
    {
        int id = touch.fingerId;
        if (EventSystem.current.IsPointerOverGameObject(id))
        {
            return true;
        }

        return false;
    }

    public Vector2 GetTouchPosition()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                currentTouch = Input.GetTouch(0);
                currentTouchPhase = currentTouch.phase;
                blockMovement = CheckIfTouchIsOverUI(currentTouch);

                if (!blockMovement)
                {
                    switch (currentTouchPhase)
                    {
                        case TouchPhase.Began:
                            previousTouchPos = currentTouch.position;
                            return currentTouch.position;
                        case TouchPhase.Moved:
                            previousTouchPos = currentTouch.position;
                            return currentTouch.position;
                        case TouchPhase.Stationary:
                            previousTouchPos = currentTouch.position;
                            return currentTouch.position;
                        case TouchPhase.Ended:
                            return previousTouchPos;
                        case TouchPhase.Canceled:
                            return previousTouchPos;
                        default:
                            return previousTouchPos;
                    }
                }
            }
        }
        else
        {
            previousTouchPos = Input.mousePosition;
        }
        return previousTouchPos;
    }

    public bool GetClickDown()
    {
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            return true;
        }

        return false;
    }

    public float GetMouseVelocity()
    {
        return Input.GetAxis(HorizontalMouse);
    }
    */
}