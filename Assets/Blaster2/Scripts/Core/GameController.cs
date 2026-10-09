using DG.Tweening;
using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEditor.Overlays;
using UnityEngine;

[Serializable]
public struct PlayerShipElement
{
    public string name;
    public GameObject prefab;
}

public enum GameState
{
    IDLE,
    SPAWN_PLAYER,
    INITIALIZING,
    START,
    TRANSMISSION,
    GAME,
    INTERMEDIATE_REWARD,
    INTERMEDIATE_REWARD_CLAIMED,
    HyperspaceTransition,
    GAMEOVER, 
    WIN
}

public class GameController : MonoBehaviour
{
    public event Action<GameState> OnGameStateValueChanged;
    public event Action<int> OnPlayerKillingStreakValueChanged;
    public Action<int> OnGameCoinsPickedValueChanged;
    public Action<int> OnGameScoreValueChanged;
    public GameState CurrentGameState { get; set; } = GameState.START;

    [Header("Config")]
    public bool IsFirstRun { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsTransmiting { get; set; }
    public bool IsSlowMo { get; set; }

    [Header("Gameplay Configuration")]
    public bool pause;
    public int Multiplier = 1;
    public int Score = 0;
    public int Currency = 0;

    [SerializeField] private PlayerShipElement[] PlayerShips;

    [SerializeField] protected CameraManager cameraManager;
    [SerializeField] protected WaveManager waveManager;
    [SerializeField] protected AsteroidSpawner asteroidSpawner;
    [SerializeField] protected GuiManager guiManager;
    [SerializeField] protected DialogueManager dialogueManager;
    [SerializeField] protected IntermediateRewardManager intermediateRewardManager;
    [SerializeField] protected LevelManager levelManager;


    private GameObject currentPlayer;
    private PlayerShip playerShip;
    private PlayerData playerData;
    private float timer = 1;

    private IDataService dataService;
    private IAudioService audioService;
    private IAppService appService;
    private IEventService eventService;

    public bool IsDebugMode = false;


    //=================================================================================
    protected void Awake()
    {
        IsSlowMo = false;
        Application.targetFrameRate = 60;
    }
    //=================================================================================
    protected void Start()
    {
        dataService = GameContext.Get<IDataService>();
        audioService = GameContext.Get<IAudioService>();
        appService = GameContext.Get<IAppService>();
        eventService = GameContext.Get<IEventService>();

        playerData = dataService.GetPlayerData();


        playerData.GetCurrentPlayerShipData().Upgrades[(int)UpgradeTypeEnum.Shield] = 0;
        waveManager.SetLevel(dataService.GetPlayerData().GetCurrentPlayerShipData().Level);

        SetGameState(GameState.SPAWN_PLAYER);

        eventService.Subscribe<EnemyEvent>(EnemyEventHandled);

        audioService.PlayMusicById("Track1");

        waveManager.OnGameplayLoopStateValueChanged += WaveManager_OnGameplayLoopStateValueChanged;
    }

    private void WaveManager_OnGameplayLoopStateValueChanged(GameplayLoopState obj)
    {
        if(obj == GameplayLoopState.BossDefeated)
        {
            SetGameState(GameState.INTERMEDIATE_REWARD);
            intermediateRewardManager.Show(() =>
            {
                SetGameState(GameState.INTERMEDIATE_REWARD_CLAIMED);
            });
        }
    }

    //=================================================================================
    private void OnDestroy()
    {
        eventService.Unsubscribe<EnemyEvent>(EnemyEventHandled);
        DOTween.Clear(true);
        DOTween.ClearCachedTweens();
    }

    //=================================================================================
    private void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.SPAWN_PLAYER:

                appService.SetGameState(TheGamerUrso.Core.GameStateEnum.GAME);

                IsGameOver = false;
                waveManager.SetLevel(1);

                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    if (GetPlayer() == null)
                    {
                        int shipSelected = playerData.CurrrentSelectedShip;
                        var player = CreatePlayer(shipSelected);
                        playerShip = player.GetComponentInChildren<PlayerShip>();
                        playerShip.weaponController.DisableFire();
                        var followPlayer = GameObject.FindAnyObjectByType<PlayerFollow>();       
                        cameraManager.SetTarget(followPlayer != null ? followPlayer.gameObject : null);
                    }
                    timer = 2;
                    SetGameState(GameState.INITIALIZING);
                }
            break;
            case GameState.INITIALIZING:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    guiManager.Setup((PlayerShip)playerShip, playerData);
                    if (IsDebugMode) return;
                    SetGameState(GameState.START);
                }
                break;
            case GameState.START:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    playerShip.weaponController.EnableFire();
                    guiManager.RecieveTransmition(Array.Empty<string>());
                    if (dialogueManager.ShowDialogue(0, () => { SetGameState(GameState.GAME); }))
                    {
                        SetGameState(GameState.TRANSMISSION);
                        timer = 5;
                    }
                    else
                    {
                        SetGameState(GameState.GAME);
                    }
                }
                break;
            case GameState.TRANSMISSION:
                break;
            case GameState.GAME:
                if(playerShip.healthComponent.CurrentHealth <= 0)
                {
                    GameOver();              
                }
                break;
            case GameState.INTERMEDIATE_REWARD:
               
                break;
            case GameState.INTERMEDIATE_REWARD_CLAIMED:
                    audioService.PlayMusic("Track1");
                    StartHyperspaceSequence();
                break;
            case GameState.HyperspaceTransition:
                break;
            case GameState.GAMEOVER:
                break;
            case GameState.WIN:
                break;
        }
    }
    //=================================================================================
    private void StartHyperspaceSequence()
    {
        levelManager.ActivateHyperdrive(OnLevelCHangedHandled);
        SetGameState(GameState.HyperspaceTransition);
    }
    //=================================================================================
    public void OnLevelCHangedHandled()
    {
        SetGameState(GameState.GAME);
    }
    //=================================================================================
    public void SetGameState(GameState nextGameState)
    {
        switch (nextGameState)
        {
            case GameState.GAMEOVER:
                guiManager.GameOver();
                break;
            case GameState.WIN:
                guiManager.GameOver();
                break;
        }
        CurrentGameState = nextGameState;
    }
    //======================================================================================================================================================
    public void GameOver()
    {
        audioService.PlayMusicById("GameOver");
        SetGameState(GameState.GAMEOVER);

        Time.timeScale = 1.0f;
        
        eventService?.Publish(new QuestProgressEvent() { questTypeEnum = QuestTypeEnum.SCORE, value = Score });
    }
    //=================================================================================
    public void ResetMultiplier()
    {
        Multiplier = 1;
    }
    //=================================================================================
    public void IncreaseMultiplier()
    {
        Multiplier++;
        if (Multiplier >= 5)
        {
            Multiplier = 5;
        }
    }
    //=================================================================================
    public void DecreaseMultipler()
    {
        Multiplier--;
        if (Multiplier < 0)
        {
            Multiplier = 0;
        }
    }
    //======================================================================================================================================================
    public void SetScore(int enemyValue)
    {
        var valueReward = enemyValue * Multiplier;
        Score += valueReward; 
        if (Score >= int.MaxValue)
        {
            Score = int.MaxValue;
        }
        var ultiplierTextToShow = Multiplier > 1 ? $"{enemyValue} + (x {Multiplier} )" : $"{enemyValue * Multiplier}";
        eventService.Publish(new FloatingTextEvent(){Message = ultiplierTextToShow ,targetPos = playerShip.transform.localPosition});
        playerData.playerStatsData.UpdateScore(valueReward);
    }
    //======================================================================================================================================================
    IEnumerator DelayGameOver()
    {
        audioService.PlayMusic("GameOver", false);
        yield return new WaitForSeconds(2.0f);
    }
    //=================================================================================
    public GameObject CreatePlayer(int id)
    {
        if (id >= PlayerShips.Length)
        {
            id = 0;
        }

        var playerShipData = dataService.GetPlayerData().GetCurrentPlayerShipData();
        currentPlayer = GameObject.Instantiate(PlayerShips[id].prefab.gameObject);
        currentPlayer.SetActive(true);

        return currentPlayer;
    }
    //=================================================================================
    public PlayerShip GetPlayer()
    {
        if (currentPlayer == null)
        {
            return null;
        }
        return currentPlayer.GetComponentInChildren<PlayerShip>();
    }
    //=================================================================================
    public virtual void EnemyEventHandled(EnemyEvent payload)
    {
        var playerData = dataService.GetPlayerData();
        switch (payload.Type)
        {
            case EnemyEvent.EnemyEventType.NONE:
                break;
            case EnemyEvent.EnemyEventType.DEATH:
                float playerLevel = playerData.GetCurrentPlayerShipData().Level;
                int enemyLevel = payload.Enemy.shipData.Level;

                RewardXP(enemyLevel);

                if (!playerShip.GetWeaponController().IsSuperActive())
                    playerData.GetCurrentPlayerShipData().UpdateSuperCharge(0.025f);

                SetScore((int)payload.Value);
            
                IncreaseMultiplier();

                playerData.playerStatsData.UpdateEnemyKilled(1);
                if (payload.Enemy.GetComponent<BossEnemy>() != null)
                {
                    playerData.playerStatsData.UpdateBounty(1);
                }
                break;
            case EnemyEvent.EnemyEventType.ESCAPE:
                DecreaseMultipler();
                break;
            case EnemyEvent.EnemyEventType.HIT:
                if (!playerShip.GetWeaponController().IsSuperActive())
                    playerData.GetCurrentPlayerShipData().UpdateSuperCharge(0.025f);
                break;
        }
    }

    public void RewardXP(int enemyLevel)
    {
        float playerLevel = playerData.GetCurrentPlayerShipData().Level;

        // 1. Calculate precise float ratio (e.g., 1.0 for equal, <1 if player is higher, >1 if enemy is higher)
        float levelRatio = enemyLevel / Mathf.Max(1f, playerLevel);

        // 2. Base XP scales with how strong the enemy is, modified by the level ratio
        float baseXP = 2f * enemyLevel;

        // 3. Clamp the multiplier so high-level players still get a tiny baseline (e.g., 10%), 
        //    and over-leveled enemies cap out at a reasonable bonus (e.g., 2x)
        float xpMultiplier = Mathf.Clamp(levelRatio, 0.1f, 2.0f);

        int xpEarned = Mathf.Max(1, Mathf.RoundToInt(baseXP * xpMultiplier));

        playerData.GetCurrentPlayerShipData().EarnXP(xpEarned);

        eventService.Publish(new FloatingTextEvent() { Message = $"<color=yellow> {xpEarned} XP </color>", targetPos = transform.localPosition });
    }
}
