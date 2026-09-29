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

    private Vector2 currentMousePosition;
    private Vector2 previousPos;
    private Vector3 targetPos;
    private Vector3 targetRotation;
    private Plane plane;
    private Ray ray;
    [SerializeField] private float rotationSpeed;
    private float hitPoint;
    private float deltaX;
    private float deltaY;
    private float offset = 8;

    private Camera _cam;
    private PlayerShipData playerShipData;
    private PlayerData playerData;

    //Mouse Click
    private int clicktimes;
    private float clicktimer;
    private bool clicked;
    private float clickDelay = .25f;

    private IDataService dataService;
    private IEventService eventService;

    [SerializeField] private WeaponController weaponController;
    [SerializeField] private ItemPickupEffect itemPickupEffect;
    private void Awake()
    {
        _cam = Camera.main;
        targetPos = transform.position;
        plane = new Plane(Vector3.up, transform.position);
    }
    //=================================================================================
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
        playerData.PowerUp(0);

        playerShip.Setup(playerData, playerShipData);
    }
    //=================================================================================
    private void Update()
    {
        if (playerShip.currentPlayerState == PlayerStateEnum.Enter || playerShip.currentPlayerState == PlayerStateEnum.Exit)
            return;

        Rotate();

#if UNITY_EDITOR
        MouseControls();
#elif UNITY_ANDROID
        TouchControls();
#elif UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR_64
       GamepadControls();
#endif
        ClampTransform();

        if (playerData.PowerPackCollected >= 5)
        {
            playerData.PowerPackCollected = 0;
            playerShip.UpgradeWeapon();
        }
    }
    //=================================================================================
    public void MouseControls()
    {
        var attackButtonPressed = (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1")) && !IsMouseOverUI();
        var moveButtonPressed = Input.GetMouseButton(0) && !IsMouseOverUI();
        var superButtonPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3"); 

        var playerPowerUp = playerData.GetPowerUpLevelPresentage();
        weaponController.ShouldAttack = Input.GetMouseButton(0);


        if (moveButtonPressed)
        {
            currentMousePosition = Input.mousePosition;
            var normlised = (currentMousePosition - previousPos).normalized;

            deltaX = normlised.x;
            deltaY = normlised.y;

            SetTargetPosition(currentMousePosition);
            MoveToTarget();          

            previousPos = currentMousePosition;
        }

    
        if (attackButtonPressed)
        {
            weaponController.GetCurrentWeapon().Shoot();
        }

        if (superButtonPressed)
        {
            if (!clicked)
            {
                clicked = true;
                clicktimer = clickDelay;
            }
            clicktimes++;
            if (clicktimes > 1)
            {
                if (playerPowerUp >= 1)
                {
                    playerShip.ActiveSpecial();
                }
            }
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
    }
    //=================================================================================
    public void TouchControls()
    {
        if (Input.touchCount > 0 && !IsMouseOverUI())
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                var currentPos = Input.GetTouch(0).position;
                var normlised = (currentPos - previousPos).normalized;

                deltaX = normlised.x;
                deltaY = normlised.y;

                transform.Translate(new Vector3((deltaX * Speed * Time.deltaTime), 0, (deltaY * Speed * Time.deltaTime)));

                previousPos = currentPos;
            }
        }

        var shouldShoot = Input.GetMouseButton(0) || Input.touchCount > 0;
        var holdFire = Input.touchCount > 1 || (Input.GetMouseButton(1) && Input.GetMouseButton(0)) ? true : false;
        if (shouldShoot && !holdFire)
        {
            weaponController.GetCurrentWeapon().Shoot();
        }

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
    }
    //=================================================================================
    public void GamepadControls()
    {
        var newPosition = transform.localPosition + new Vector3((Input.GetAxis("Horizontal") * (Speed / 1.25f) * Time.deltaTime), 0, (Input.GetAxis("Vertical") * (Speed / 1.25f) * Time.deltaTime));
        rotationSpeed = -Input.GetAxis("Horizontal") * tilt;
        rotationSpeed = Mathf.Clamp(rotationSpeed, -35, 35);
        targetRotation = new Vector3(0, 0, rotationSpeed);
        transform.SetPositionAndRotation(newPosition, transform.transform.rotation);
        ShipModel.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(targetRotation));
    }
    //=================================================================================
    public void SetTargetPosition(Vector2 screenPos)
    {
        ray = _cam.ScreenPointToRay(screenPos);

        if (plane.Raycast(ray, out hitPoint))
            targetPos = new Vector3(ray.GetPoint(hitPoint).x, 0, ray.GetPoint(hitPoint).z);
    }
    //=================================================================================
    public void Rotate()
    {
        Quaternion targetRotation = ShouldRoate() ? Quaternion.Euler(0f, 0f, -deltaX * tilt) : Quaternion.Euler(0f, 0f, 0f);

        ShipModel.transform.localRotation = Quaternion.Slerp(
            ShipModel.transform.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
    //=================================================================================
    private void MoveToTarget() => transform.position = Vector3.MoveTowards(transform.position, targetPos + new Vector3(0, 0, offset), Speed * Time.deltaTime);
    //=================================================================================
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    //=================================================================================
    public bool ShouldRoate() => (Input.touchCount > 0 || Input.GetMouseButton(0));
    //=================================================================================
    public void ClampTransform() => transform.position = new Vector3(Mathf.Clamp(transform.position.x, Constants.m_XMin, Constants.m_XMax), 0, Mathf.Clamp(transform.position.z, 0, 120));

    //=================================================================================
    public void SetWallet(int coin)
    {
        playerData.AddCoin(coin);
        PlayerPrefs.SetInt("CoinTut", 1);
        eventService.Publish(new FloatingTextEvent() { Message = $"<color=yellow> {coin} $ </color>", targetPos = transform.localPosition });
    }
    //=================================================================================
    public void OnRewardItemHandled(RewardItemEvent payload)
    {
        switch (payload.rewardType)
        {
            case RewardTypeEnum.Gold:
                playerData.AddCoin((int)payload.reward);
                eventService.Publish(new FloatingTextEvent() { Message = $"<color=yellow> {payload.reward} Coin </color>", targetPos = transform.localPosition });
                break;
            case RewardTypeEnum.XP:
                var xpReward = Mathf.Clamp((float)payload.reward, 1, playerShipData.xpToLevel);
                playerData.GetCurrentPlayerShipData().EarnXP(xpReward);
                eventService.Publish(new FloatingTextEvent() { Message = $"<color=yellow> {xpReward} XP </color>", targetPos = transform.localPosition });
                break;
            case RewardTypeEnum.HEALTH:
                var damagable = playerShip.GetComponent<IDamagable>();
                damagable.Heal(playerShipData.Health / 2);
                break;
            case RewardTypeEnum.SHIELD:
                playerShip.ActiveSpecial();
                break;
            case RewardTypeEnum.POWERUP:
                playerData.PowerUp(2);
                break;
            case RewardTypeEnum.SUPER:
                float power = playerData.ChargePower + .5f;
                playerData.SetSuperMeter(power);
                break;
        }
    }
    //=================================================================================
    public void OnLevelValueChanged(int Level)
    {
        playerShip.SetStats(playerShipData.level, playerShipData.Health, playerShipData.Speed, playerShipData.Damage, playerShipData.FireRate, playerShipData.GetCalculatedUpgradeStats(), playerShipData.SuperDamage, playerShipData.SuperChargeTime);
    }
    //=================================================================================
    public void OnItemPickedUpHandled(ItemPickedUpEvent payload)
    {
        switch (payload.ItemType)
        {
            case ItemEnum.COIN:
                SetWallet((int)payload.Ammount);
                break;
            case ItemEnum.SHIELD:
                playerShip.ActiveShield();
                itemPickupEffect.Show(0);
                break;
            case ItemEnum.POWERUP:
                bool canUseItem = playerShip.playerStats.CanUsePowerUpItem;
                itemPickupEffect.Show(1);
                if (canUseItem)
                {
                    if (weaponController.CurrentWeaponIndex < 4)
                    {
                        playerData.PowerUp(2);
                    }
                    else
                    {
                        playerData.SetSuperMeter(playerData.ChargePower + 0.025f);
                    }

                }
                else if (!canUseItem)
                {
                    playerData.SetSuperMeter(playerData.ChargePower + 0.025f);
                }
                break;
            case ItemEnum.HEALTH:
                playerShip.healthComponent.Heal(((float)payload.Ammount) * playerShipData.level);
                itemPickupEffect.Show(2);
                break;
            case ItemEnum.EMPTY:
                break;
            default:
                break;
        }
    }
}
