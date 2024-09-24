using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : StateMachine
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    
    [Header("Scriptable Objects")]
    [SerializeField] private EnemySpawnServiceScriptableObject enemySpawnServiceScriptableObject;
    [SerializeField] private ObjectPoolServiceScriptableObject objectPoolServiceScriptableObject;
    [SerializeField] private PickupServiceScriptableObject pickupServiceScriptableObject;
    
    [Header("Script References")]
    public PlayerController PlayerController;
    public WeaponController PlayerWeaponController;
    public UIService uiService;
    public EventService EventService;
    public ObjectPoolingService ObjectPoolingService;
    
    private EnemySpawnService enemySpawnService;
    private PickupSpawnService pickupSpawnService;
    public GamePauseType currentGamePauseType { get; private set; }

    private void Awake()
    {
        Init();
        InitializeServices();
        SubscribeToEvents();
        AddStates();
    }

    private void Init()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void InitializeServices()
    {
        EventService = new EventService();
        ObjectPoolingService = new ObjectPoolingService(objectPoolServiceScriptableObject);
        enemySpawnService = new EnemySpawnService(enemySpawnServiceScriptableObject);
        pickupSpawnService = new PickupSpawnService();
        uiService.Init();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        EventService.OnGameEnteredPlayState += SwitchState <PlayingState>;
        EventService.OnGameEnteredPauseState += SwitchState <PausedState>;
        EventService.OnPlayerSelectedUpgrade += SwitchState<PlayingState>;
    }

    private void UnsubscribeFromEvents()
    {
        EventService.OnGameEnteredPlayState -= SwitchState <PlayingState>;
        EventService.OnGameEnteredPauseState -= SwitchState <PausedState>;
        EventService.OnPlayerSelectedUpgrade -= SwitchState<PlayingState>;
    }
    protected override void AddStates()
    {
        states = new List<State>();
        states.Add(new PlayingState(enemySpawnService, PlayerController, this));
        states.Add(new PausedState(enemySpawnService, PlayerController, this));
    }

    private void Start()
    {
        EventService.InvokeGameEnteredPlayStateEvent();
    }

    private void Update()
    {
        UpdateStateMachine();
    }
    
    public void ChangeGamePauseType(GamePauseType gamePauseType) => currentGamePauseType = gamePauseType;
}
