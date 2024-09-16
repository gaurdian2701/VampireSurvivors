using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public PlayerController Player;
    public EventService EventService;
    public ObjectPoolingService ObjectPoolingService;

    [SerializeField] private EnemySpawnServiceScriptableObject enemySpawnServiceScriptableObject;
    [SerializeField] private ObjectPoolServiceScriptableObject objectPoolServiceScriptableObject;

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
    }

    private void Start()
    {
        enemySpawnService = new EnemySpawnService(enemySpawnServiceScriptableObject);
    }
}
