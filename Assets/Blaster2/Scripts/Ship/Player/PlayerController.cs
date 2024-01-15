using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance
    {
        get; private set;
    }
    public enum ControlSceme
    {
        CONTROL1 = 1, CONTROL2 = 2
    }
    public ControlSceme controlSceme;

    private Camera _cam;

    private PlayerShipData playerShipData;
    private PlayerData playerData;

    [SerializeField] private float tilt;
    [SerializeField] private GameObject ShipModel;
    private Vector3 targetPos;
    private Plane plane;
    private Ray ray;
    private float offset = 8;

    [SerializeField] private float Speed = 50;

    private float rotationSpeed;

    private Vector3 targetRotation;

    private float hitPoint;

    private Vector2 currentPos;
    private Vector2 previousPos;

    private float deltaX;
    private float deltaY;

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }

    void LoadPlayerData()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        controlSceme = (ControlSceme)playerData.ControlScene;
        Speed = playerShipData.Speed;
    }
    private void Awake()
    {
        _cam = Camera.main;
        Instance = this;
    }

    public void Start()
    {
        targetPos = transform.position;
        plane = new Plane(Vector3.up, transform.position);
        Events.OnControlScemeChange = UpdateOffset;
        LoadPlayerData();

        controlSceme = ControlSceme.CONTROL1;
    }

    public void SetTargetPosition(Vector2 screenPos)
    {
        ray = _cam.ScreenPointToRay(screenPos);

        if (plane.Raycast(ray, out hitPoint))
            targetPos = new Vector3(ray.GetPoint(hitPoint).x, 0, ray.GetPoint(hitPoint).z);
    }



    public void OnDragMove()
    {
        #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                currentPos = Input.mousePosition;
                var normlised = (currentPos - previousPos).normalized;

                deltaX = normlised.x;
                deltaY = normlised.y;

                transform.Translate(new Vector3((deltaX * Speed * Time.deltaTime), 0, (deltaY * Speed * Time.deltaTime)));

                previousPos = currentPos;
            }
        #elif UNITY_ANDROID
         if (Input.touchCount > 0 && !IsMouseOverUI())
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    currentPos = Input.GetTouch(0).position;
                    var normlised = (currentPos - previousPos).normalized;

                    deltaX = normlised.x;
                    deltaY = normlised.y;

                    transform.Translate(new Vector3((deltaX * Speed * Time.deltaTime), 0, (deltaY * Speed * Time.deltaTime)));

                    previousPos = currentPos;
                }
            }
            #endif
    }
    private void Update()
    {
        if (GameController.CurrentGameState == GameController.GameState.GAME)
        {
 #if UNITY_EDITOR_64
             if (Input.GetMouseButton(0) && !IsMouseOverUI())
             {
                 SetTargetPosition(Input.mousePosition);
                MoveToTarget();
            }
 #elif UNITY_ANDROID || UNITY_EDITOR_64
            if (controlSceme == ControlSceme.CONTROL1)
            {
                if (Input.touchCount > 0 && !IsMouseOverUI())
                {
                    Touch firstTouch = Input.GetTouch(0);
                    SetTargetPosition(firstTouch.position);
                    MoveToTarget();
                }
                if (!GameManager.Instance.IsPaused)
                {
                    Rotate();
                }
            }

#elif UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR_64
            GamepadControls();
#endif
            ClampTransform();
        }
    }

    private void FixedUpdate()
    {
#if UNITY_ANDROID
        if (controlSceme == ControlSceme.CONTROL2)
        {
            OnDragMove();
        }
#endif
    }

    public void GamepadControls()
    {
        var newPosition = transform.localPosition + new Vector3((Input.GetAxis("Horizontal") * (Speed / 1.25f) * Time.deltaTime), 0, (Input.GetAxis("Vertical") * (Speed / 1.25f) * Time.deltaTime));
        rotationSpeed = -Input.GetAxis("Horizontal") * tilt;
        rotationSpeed = Mathf.Clamp(rotationSpeed, -35, 35);
        targetRotation = new Vector3(0, 0, rotationSpeed);
        transform.SetPositionAndRotation(newPosition, transform.transform.rotation);
        ShipModel.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(targetRotation));
    }

    public void Rotate()
    {
        if (ShouldRoate())
        {
            rotationSpeed = -deltaX * tilt;
            rotationSpeed = Mathf.Clamp(rotationSpeed, -35, 35);
        }
        else
        {
            rotationSpeed = 0;
        }

        targetRotation = ShipModel.transform.localEulerAngles;
        targetRotation = new Vector3(rotationSpeed, 0, 0);
        ShipModel.transform.localEulerAngles = targetRotation;
    }

    private void MoveToTarget() => transform.position = Vector3.MoveTowards(transform.position, targetPos + new Vector3(0, 0, offset), Speed * Time.deltaTime);
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    public void UpdateOffset() => controlSceme = (ControlSceme)playerData.ControlScene;
    public bool ShouldRoate() => (Input.touchCount > 0 || Input.GetMouseButton(0));
    public void ClampTransform() => transform.position = new Vector3(Mathf.Clamp(transform.position.x, Constants.m_XMin, Constants.m_XMax), 0, Mathf.Clamp(transform.position.z, 0, 120));

}