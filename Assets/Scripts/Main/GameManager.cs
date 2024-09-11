using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public PlayerController Player;
    public Camera MainCamera;
    public EventService EventService;

    [SerializeField] private EnemySpawnServiceScriptableObject enemySpawnServiceScriptableObject;

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
        enemySpawnService = new EnemySpawnService(enemySpawnServiceScriptableObject, MainCamera);
    }

    private void Update()
    {

    }
}
