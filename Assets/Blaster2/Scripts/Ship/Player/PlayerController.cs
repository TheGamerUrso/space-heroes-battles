using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.Audio;
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
    [SerializeField] private float rotationSpeed;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private ItemPickupEffect itemPickupEffect;

    private Camera _cam;

    private Vector2 currentMousePosition;
    private Vector2 previousPos;
    private Vector3 targetPos;
    private Vector3 targetRotation;
    private Plane plane;
    private Ray ray;
    private float hitPoint;
    private float deltaX;
    private float deltaY;
    private float offset = 8;

    private int clicktimes;
    private float delayClickTimer;
    private bool HasClicked;
    private float clickDelayTime = .25f;

    private IDataService dataService;
    private IEventService eventService;

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

        var playerData = dataService.GetPlayerData();
        var shipData = playerData.GetCurrentPlayerShipData();
        Speed = shipData.Speed;

        controlScemeEnum = (ControlScemeEnum)playerData.playerSettingsData.ControlScene;
        playerShip.Setup(shipData);

        eventService.Subscribe<PlayerStatsUpdatedEvent>(PlayerStatsUpdateEventHandled);
        eventService.Subscribe<ItemPickedUpEvent>(OnItemPickedUpHandled);
        eventService.Subscribe<RewardItemEvent>(OnRewardItemHandled);
    }

   

    //=================================================================================
    private void OnDestroy()
    {
        eventService.Unsubscribe<PlayerStatsUpdatedEvent>(PlayerStatsUpdateEventHandled);
        eventService.Unsubscribe<ItemPickedUpEvent>(OnItemPickedUpHandled);
        eventService.Unsubscribe<RewardItemEvent>(OnRewardItemHandled);
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
    }
    //=================================================================================
    public void MouseControls()
    {
        var attackButtonPressed = (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1")) && !IsMouseOverUI();
        var moveButtonPressed = Input.GetMouseButton(0) && !IsMouseOverUI();
        var superButtonPressed = Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3");
        var holdFireButtonPressed = Input.touchCount > 1 || (Input.GetMouseButton(1) && Input.GetMouseButton(0)) ? true : false;
        var playerPowerUp = playerShip.GetPlayerShipData().ChargePower/ 100;
        weaponController.ShouldAttack = Input.GetMouseButton(0);


        if (moveButtonPressed)
        {
            currentMousePosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                previousPos = currentMousePosition;
            }

            var normlised = (currentMousePosition - previousPos).normalized;

            deltaX = normlised.x;
            deltaY = normlised.y;

            SetTargetPosition(currentMousePosition);
            MoveToTarget();          

            previousPos = currentMousePosition;
        }

    
        if (attackButtonPressed && !holdFireButtonPressed)
        {
            weaponController.GetCurrentWeapon().Shoot();
        }

        if (HasClicked)
        {
            delayClickTimer -= Time.deltaTime;
            if (delayClickTimer <= 0)
            {
                HasClicked = false;     
 
            }
        }

        if (!HasClicked && superButtonPressed)
        {
            if (!HasClicked)
            {
                HasClicked = true;
                delayClickTimer = clickDelayTime;
            }
            ((PlayerWeaponController)weaponController).ActivateSpecial();
        }               
    }
    //=================================================================================
    public void TouchControls()
    {
        var attackButtonPressed = (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1")) && !IsMouseOverUI();
        var moveButtonPressed = Input.touchCount > 0 && !IsMouseOverUI();
        var superButtonPressed = Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3");
        var holdFireButtonPressed = Input.touchCount > 1 || (Input.GetMouseButton(1) && Input.GetMouseButton(0)) ? true : false;
        var playerPowerUp = playerShip.GetPlayerShipData().ChargePower / 100;

        if (moveButtonPressed)
        {
            var currentTouch = Input.GetTouch(0);

            if (currentTouch.phase == TouchPhase.Moved)
            {
                var currentPos = currentTouch.position;
                var normlised = (currentPos - previousPos).normalized;

                deltaX = normlised.x;
                deltaY = normlised.y;

                transform.Translate(new Vector3((deltaX * Speed * Time.deltaTime), 0, (deltaY * Speed * Time.deltaTime)));

                previousPos = currentPos;
            }
        }

        if (attackButtonPressed && !holdFireButtonPressed)
        {
            weaponController.GetCurrentWeapon().Shoot();
        }

        if (Time.timeScale == 0)
        {
            HasClicked = false;
            delayClickTimer = 1;
            clicktimes = 0;
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            clicktimes = touch.tapCount;
        }

        if (superButtonPressed && playerPowerUp >= 1)
        {
            if (!HasClicked)
            {
                HasClicked = true;
                delayClickTimer = clickDelayTime;
            }
            clicktimes++;
            if (clicktimes > 1)
            {
                ((PlayerWeaponController)weaponController).ActivateSpecial();
            }
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
    public void OnRewardItemHandled(RewardItemEvent payload)
    {
        switch (payload.rewardType)
        {
            case RewardTypeEnum.Gold:
                eventService.Publish(new PlayerEconomyDataUpdatedEvent() { type = PlayerEconomyDataUpdatedEvent.StatType.Coins, value = (int)payload.reward });
                eventService.Publish(new FloatingTextEvent() { Message = $"<color=yellow> {payload.reward} Coin </color>", targetPos = transform.localPosition });
                break;
            case RewardTypeEnum.XP:
                eventService.Publish(new PlayerStatsUpdatedEvent() { type = PlayerStatsUpdatedEvent.StatType.XP, value = (int)payload.reward });
          
                var playerShipData = playerShip.GetPlayerShipData();
                if (playerShipData == null) return;

                var xpReward = Mathf.Clamp((float)payload.reward, 1, playerShipData.xpToLevel);
                playerShipData.EarnXP(xpReward);
                eventService.Publish(new FloatingTextEvent() { Message = $"<color=yellow> {xpReward} XP </color>", targetPos = transform.localPosition });
                break;
            case RewardTypeEnum.HEALTH:
                var damagable = playerShip.GetComponent<IDamagable>();
                damagable.Heal(playerShip.healthComponent.CurrentHealth / 2);
                break;
            case RewardTypeEnum.SHIELD:
                playerShip.ActiveShield();
                break;
            case RewardTypeEnum.POWERUP:
                playerShip.GetPlayerShipData().UpdatePowerPackCollected(2); 
                break;
            case RewardTypeEnum.SUPER:
                playerShip.GetPlayerShipData().UpdateUpserCharge(.5f);
                break;
        }
    }
    //=================================================================================
    public void OnLevelValueChanged(int Level)
    {
        playerShip.SetStats(Level);
        playerShip.ApplyUpgrades(playerShip.GetPlayerShipData().GetCalculatedUpgradeStats());
    }
    //=================================================================================
    public void OnItemPickedUpHandled(ItemPickedUpEvent payload)
    {
        switch (payload.ItemType)
        {
            case ItemEnum.COIN:
                eventService.Publish(new PlayerEconomyDataUpdatedEvent() { type = PlayerEconomyDataUpdatedEvent.StatType.Coins, value = (int)payload.Ammount });

                PlayerPrefs.SetInt("CoinTut", 1);
                eventService.Publish(new FloatingTextEvent() { Message = $"<color=yellow> {(int)payload.Ammount} $ </color>", targetPos = transform.localPosition });
                break;
            case ItemEnum.SHIELD:
                playerShip.ActiveShield();
                itemPickupEffect.Show(0);
                break;
            case ItemEnum.POWERUP:
                bool canUseItem = playerShip.GetPlayerSO().CanUsePowerUpItem;
                itemPickupEffect.Show(1);
                if (canUseItem)
                {
                    if (weaponController.CurrentWeaponIndex < 4)
                    {
                        playerShip.GetPlayerShipData().UpdatePowerPackCollected(1);
                    }
                    else
                    {
                        playerShip.GetPlayerShipData().UpdateUpserCharge(0.025f);
                    }

                }
                else if (!canUseItem)
                {
                    playerShip.GetPlayerShipData().UpdateUpserCharge(0.025f);
                }
                break;
            case ItemEnum.HEALTH:
                playerShip.healthComponent.Heal(((float)payload.Ammount) * playerShip.GetPlayerShipData().Level);
                itemPickupEffect.Show(2);
                break;
        }
    }
    //=================================================================================
    public void PlayerStatsUpdateEventHandled(PlayerStatsUpdatedEvent payload)
    {
        switch (payload.type)
        {
            case PlayerStatsUpdatedEvent.StatType.ChargePower:
                 playerShip.GetPlayerShipData().UpdateUpserCharge(payload.value);
                break;
            case PlayerStatsUpdatedEvent.StatType.PowerPackCollected:
                playerShip.GetPlayerShipData().UpdatePowerPackCollected((int)payload.value);
                break;
        }

    }    
}
