using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : StateMachine
{
    public float gameSpeed;
    
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    
    [Header("Scriptable Objects")]
    [SerializeField] private EnemySpawnServiceScriptableObject enemySpawnServiceScriptableObject;
    [FormerlySerializedAs("objectPoolServiceScriptableObject")] [SerializeField] private ObjectPoolingServiceScriptableObject objectPoolingServiceScriptableObject;
    [SerializeField] private PickupServiceScriptableObject pickupServiceScriptableObject;
    
    [Header("Script References")]
    public PlayerController PlayerController;
    [FormerlySerializedAs("PlayerWeaponController")] public WeaponsManager playerWeaponsManager;
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
        ObjectPoolingService = new ObjectPoolingService(objectPoolingServiceScriptableObject);
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
        EventService.OnPlayerDied += GameOver;
    }

    private void UnsubscribeFromEvents()
    {
        EventService.OnGameEnteredPlayState -= SwitchState <PlayingState>;
        EventService.OnGameEnteredPauseState -= SwitchState <PausedState>;
        EventService.OnPlayerSelectedUpgrade -= SwitchState<PlayingState>;
        EventService.OnPlayerDied -= GameOver;
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
        Time.timeScale = gameSpeed;
        UpdateStateMachine();
    }

    private void GameOver()
    {
        PlayerController.gameObject.SetActive(false);
    }
    public void ChangeGamePauseType(GamePauseType gamePauseType) => currentGamePauseType = gamePauseType;
    public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void QuitGame()
    {
        PlayerPrefs.SetInt("HIGH SCORE MAX LEVEL", PlayerController.CurrentPlayerLevel);
        SceneManager.LoadScene((int)SceneBuildIndices.MAIN_MENU_SCENE);
    }
}
