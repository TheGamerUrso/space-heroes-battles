using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class SimpleShipControls : MonoBehaviour
{
    public enum ControlSceme
    {
        CONTROL1 = 1, CONTROL2 = 2
    }
    public ControlSceme controlSceme;

    private PlayerShipData playerShipData;
    private PlayerData playerData;

    [SerializeField] private float tilt;

    [SerializeField] private GameObject ShipModel;
    private Vector3 targetPos;
    private Plane plane;
    private Ray ray;
    private float offset = 8;
    private Touch touch;
    private Vector2 currentTouchPos;
    private float yMove = 0;
    private float movementSensitivity = .1f;
    private Vector3 direction;
    private float rotVelocity;
    private Vector3 targetEulerAngels;
    private float point;
    [SerializeField] private float Speed = 5;

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }

    public void Start()
    {
        targetPos = transform.position;

        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();


        plane = new Plane(Vector3.up, transform.position);

        Events.OnControlScemeChange = UpdateOffset;
        Speed = playerShipData.Speed;
        controlSceme = (ControlSceme)playerData.ControlScene;
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
                touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        currentTouchPos = touch.position;
                        break;
                    case TouchPhase.Moved:
                        currentTouchPos = touch.position;
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

        plane = new Plane(Vector3.up, transform.position);
        point = 0f;
        if (plane.Raycast(ray, out point))
            targetPos = new Vector3(ray.GetPoint(point).x, yMove, ray.GetPoint(point).z);
    }

    public AnimationCurve animationCurve;

    private void Move()
    {
        transform.position = Vector3.Lerp(transform.position, (targetPos + new Vector3(0, 0, offset)), Speed * Time.deltaTime);
    }

    public void OnDragMove()
    {
        if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && !IsMouseOverUI())
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                transform.position = new Vector3(transform.position.x + Input.GetAxis("Mouse X") * Speed * Time.deltaTime,
                    transform.position.y,
                    transform.position.z + Input.GetAxis("Mouse Y") * Speed * Time.deltaTime);
            }
#endif
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 targetPos = transform.position;
                    targetPos.x += touch.deltaPosition.x * Speed * Time.deltaTime;
                    targetPos.z += touch.deltaPosition.y * Speed * Time.deltaTime;
                    transform.position = targetPos;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (GameController.CurrentGameState == GameController.GameState.GAME)
        {
            if (controlSceme == ControlSceme.CONTROL1)
            {
                Move();
            }
            else if (controlSceme == ControlSceme.CONTROL2)
            {
                OnDragMove();
            }

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, Constants.m_XMin, Constants.m_XMax),
                yMove,
                    Mathf.Clamp(transform.position.z, Constants.m_ZMin, Constants.m_ZMax));
        }
    }

    private void Update()
    {
        if (GameController.CurrentGameState == GameController.GameState.GAME)
        {
            if (controlSceme == ControlSceme.CONTROL1)
            {
                if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && !IsMouseOverUI())
                {
                    SetTargetPosition();
                }
            }

            if (!Game.IsPaused)
            {
                Rotate();
            }
        }
    }


    public void Rotate()
    {
        if (ShouldRoate())
        {

#if UNITY_EDITOR
            rotVelocity = -(Input.GetAxis("Mouse X")) * tilt;
#endif

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                rotVelocity = -touch.deltaPosition.x * tilt;
            }

            rotVelocity = Mathf.Clamp(rotVelocity, -35, 35);
        }
        else
        {
            rotVelocity = 0;
        }

        targetEulerAngels = ShipModel.transform.localEulerAngles;

        targetEulerAngels = new Vector3(
              targetEulerAngels.x
            , targetEulerAngels.y,
              Mathf.LerpAngle(
                targetEulerAngels.z,
           rotVelocity, .1f));

        ShipModel.transform.localEulerAngles = targetEulerAngels;
    }

    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void UpdateOffset()
    {
        controlSceme = (ControlSceme)playerData.ControlScene;
    }

    public bool ShouldRoate()
    {
        return (Input.touchCount > 0 || Input.GetMouseButton(0));
    }
}