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
    
    [Header("Script References")]
    [SerializeField] public PlayerController PlayerController;
    [SerializeField] public UIService uiService;
    
    public EventService EventService;
    public ObjectPoolingService ObjectPoolingService;
    
    
    private EnemySpawnService enemySpawnService;

    private void Awake()
    {
        Init();
        InitializeServices();
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
        uiService.Init();
        SubscribeToEvents();
        AddStates();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        EventService.OnGameEnteredPlayState += SwitchState <PlayingState>;
        EventService.OnGameEnteredPauseState += SwitchState <PausedState>;
    }

    private void UnsubscribeFromEvents()
    {
        EventService.OnGameEnteredPlayState -= SwitchState <PlayingState>;
        EventService.OnGameEnteredPauseState -= SwitchState <PausedState>;
    }
    protected override void AddStates()
    {
        states = new List<State>();
        states.Add(new PlayingState(enemySpawnService, PlayerController));
        states.Add(new PausedState(enemySpawnService, PlayerController));
    }

    private void Start()
    {
        EventService.InvokeGameEnteredPlayStateEvent();
    }

    private void Update()
    {
        UpdateStateMachine();
    }
}
