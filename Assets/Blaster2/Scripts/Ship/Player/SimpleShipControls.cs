using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleShipControls : MonoBehaviour
{
    public enum ControlSceme
    {
        CONTROL1, CONTROL2
    }
    public ControlSceme controlSceme;

    private PlayerShipData playerShipData;
    private PlayerData playerData;

    [SerializeField] private float tilt;

    [SerializeField] private GameObject ShipModel;
    private Vector3 targetPos;
    private Plane plane;
    private Ray ray;
    private Vector3 offspec;
    private Touch currentTouch;
    private Vector2 currentTouchPos;
    private float yMove = 0;
    private float movementSensitivity = .1f;
    private Vector3 direction;
    private float rotVelocity;
    private Vector3 targetEulerAngels;
    private float point;

    public void Start()
    {
        targetPos = transform.position;

        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        offspec = new Vector3(0, 0, playerData.Distance);
        plane = new Plane(Vector3.up, transform.position);

        Events.OnDistanceValueChanged = UpdateOffset;
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

        plane = new Plane(Vector3.up, transform.position);
        point = 0f;
        if (plane.Raycast(ray, out point))
            targetPos = new Vector3(ray.GetPoint(point).x, yMove, ray.GetPoint(point).z);
    }

    private void Move()
    {
        if (direction.magnitude > .1f)
        {
            transform.Translate((direction + offspec) * playerShipData.Speed * movementSensitivity * Time.deltaTime, Space.World);
        }
    }

    public void OnDragMove()
    {
        if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && !IsMouseOverUI())
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                transform.position += new Vector3(
                    Input.GetAxis("Mouse X") * playerShipData.Speed * movementSensitivity * Time.deltaTime,
                    transform.position.y,
                    Input.GetAxis("Mouse Y") * playerShipData.Speed * Time.deltaTime);
            }
#endif
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position += new Vector3(touch.deltaPosition.x * playerShipData.Speed * Time.deltaTime, 0, touch.deltaPosition.y * playerShipData.Speed * Time.deltaTime);
                }
            }
        }
    }

    private void Update()
    {
        if (GameController.CurrentGameState == GameController.GameState.GAME)
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

            if (Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                Game.SlowMo = false;
            }
            else
            {
                Game.SlowMo = true;
            }
        }
    }

    private void LateUpdate()
    {
        if (GameController.CurrentGameState == GameController.GameState.GAME)

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

    public void Rotate()
    {
        if (ShouldRoate())
        {
            rotVelocity = -(Input.GetAxis("Mouse X")) * tilt;
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

    public void UpdateOffset(float ammount)
    {
        offspec = new Vector3(0, 0, ammount);
    }

    public bool ShouldRoate()
    {
        return (Input.touchCount > 0 || Input.GetMouseButton(0));
    }
}