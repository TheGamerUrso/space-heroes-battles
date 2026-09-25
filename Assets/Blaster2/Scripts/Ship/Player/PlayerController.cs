using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ControlScemeEnum
{
    CONTROL1 = 1, CONTROL2 = 2
}

public class PlayerController : BaseMovementController
{
    public ControlScemeEnum controlScemeEnum { get; set; }

    [SerializeField] private float tilt;
    [SerializeField] private PlayerShip playerShip;
    [SerializeField] private GameObject ShipModel;

    private Vector2 currentPos;
    private Vector2 previousPos;
    private Vector3 targetPos;
    private Vector3 targetRotation;
    private Plane plane;
    private Ray ray;
    private float rotationSpeed;
    private float hitPoint;
    private float deltaX;
    private float deltaY;
    private float offset = 8;

    private Camera _cam;
    private PlayerShipData playerShipData;
    private PlayerData playerData;

    private int clicktimes;
    private float clicktimer;
    private bool clicked;
    float clickDelay = .25f;

    private IDataService dataService;
    private IEventService eventService;

    private void Awake()
    {
        _cam = Camera.main;
        targetPos = transform.position;
        plane = new Plane(Vector3.up, transform.position);
    }

    public void Start()
    {
        dataService = GameContext.Get<IDataService>();
        eventService = GameContext.Get<IEventService>();

        eventService.Subscribe<ItemPickedUpEvent>(OnItemPickedUpHandled);
        eventService.Subscribe<RewardItemEvent>(OnRewardItemHandled);

        playerData = dataService.GetPlayerData();

        playerShipData = playerData.GetCurrentPlayerShipData();
        Speed = playerShipData.Speed;


        controlScemeEnum = (ControlScemeEnum)playerData.ControlScene;
        playerData.SetSuperMeter(0);
        playerData.SetPowerPackCollected(0);

    }

    public void OnItemPickedUpHandled(ItemPickedUpEvent payload)
    {
        switch (payload.ItemType)
        {
            case ItemEnum.COIN:
                SetWallet((int)payload.Ammount);
                break;
            case ItemEnum.SHIELD:
                //playerShip.ActiveShield();
                playerShip.OnItemPickedUp?.Invoke(0);
                break;
            case ItemEnum.POWERUP:
                playerShip.PowerUpCollected();
                playerShip.OnItemPickedUp?.Invoke(1);
                break;
            case ItemEnum.HEALTH:
                //playerShip.Heal(((float)payload.Ammount) * playerShipData.level);
                playerShip.OnItemPickedUp?.Invoke(2);
                break;
            case ItemEnum.EMPTY:
                break;
            default:
                break;
        }
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
        if (playerShip.currentPlayerState == PlayerStateEnum.Enter || playerShip.currentPlayerState == PlayerStateEnum.Exit)
        {
            return;
        }

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
#if UNITY_ANDROID
        if (Time.timeScale == 0)
        {
            clicked = false;
            clicktimer = 1;
            clicktimes = 0;
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            clicktimes = touch.tapCount;
        }
#elif UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR_64
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3"))
        {
            if (!clicked)
            {
                clicked = true;
                clicktimer = clickDelay;
            }
            clicktimes++;
        }


        if (clicked)
        {
            clicktimer -= Time.deltaTime;

            if (clicktimer <= 0)
            {
                clicked = false;
                clicktimes = 0;
            }
        }
#endif

        if (clicktimes > 1)
        {
            var playerPowerUp = playerData.GetPowerUpLevelPresentage();
            if (playerPowerUp >= 1)
            {
                playerShip.ActiveSpecial();
            }
        }



        if (playerData.PowerPackCollected >= 5)
        {
            playerData.PowerPackCollected = 0;
            playerShip.UpgradeWeapon();
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
    public void UpdateOffset() => controlScemeEnum = (ControlScemeEnum)playerData.ControlScene;
    public bool ShouldRoate() => (Input.touchCount > 0 || Input.GetMouseButton(0));
    public void ClampTransform() => transform.position = new Vector3(Mathf.Clamp(transform.position.x, Constants.m_XMin, Constants.m_XMax), 0, Mathf.Clamp(transform.position.z, 0, 120));

    //=================================================================================
    public void SetWallet(int coin)
    {
        playerData.AddCoin(coin);
        GuiManager.CreateFloatingText("<color=" + "yellow" + "> $ </color>", transform.localPosition);
        {
            PlayerPrefs.SetInt("CoinTut", 1);
        }
    }

    public void OnRewardItemHandled(RewardItemEvent payload)
    {
        switch (payload.rewardType)
        {
            case RewardTypeEnum.Gold:
                playerData.AddCoin((int)payload.reward);
                break;
            case RewardTypeEnum.XP:
                var xpReward = Mathf.Clamp((float)payload.reward, 1, playerShipData.xpToLevel);
                playerData.GetCurrentPlayerShipData().EarnXP(xpReward);
                break;
            case RewardTypeEnum.HEALTH:
                var damagable = playerShip.GetComponent<IDamagable>();
                damagable.Heal(playerShipData.Health / 2);
                break;
            case RewardTypeEnum.SHIELD:
                playerShip.ActiveSpecial();
                break;
            case RewardTypeEnum.POWERUP:
                playerShip.PowerUpCollected();
                break;
            case RewardTypeEnum.SUPER:
                float power = playerData.PowerUpLevel + .5f;
                playerData.SetSuperMeter(power);
                break;
        }
    }

    //=================================================================================
    public void OnLevelValueChanged(int Level)
    {
        playerShip.SetStats(playerShipData.level, playerShipData.Health, playerShipData.Speed, playerShipData.Damage, playerShipData.FireRate);
    }
}