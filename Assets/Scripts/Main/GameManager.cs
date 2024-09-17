using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] private EnemySpawnServiceScriptableObject enemySpawnServiceScriptableObject;
    [SerializeField] private ObjectPoolServiceScriptableObject objectPoolServiceScriptableObject;
    
    [FormerlySerializedAs("Player")] public PlayerController PlayerController;
    public EventService EventService;
    public ObjectPoolingService ObjectPoolingService;
    
    
    private EnemySpawnService enemySpawnService;
    private GameStateMachine gameStateMachine;
    private GameState currentGameState;

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
        gameStateMachine = new GameStateMachine(enemySpawnService, PlayerController);
    }

    private void Start()
    {
        EventService.InvokeGameEnteredPlayStateEvent();
    }

    private void Update()
    {
        gameStateMachine.UpdateStateMachine();
    }
}
