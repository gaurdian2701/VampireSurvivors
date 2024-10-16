using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnService : IPausable
{
    private EnemyController enemyToBeSpawned;
    private Transform playerTransform;
    private int startingNumberOfEnemies;
    private int numberOfEnemiesInHorde;
    
    private const float spawnCircleRadius = 15f;
    
    private int currentNumberOfKillsToInitiateHorde;
    private int currentKillCountForHorde;
    private int hordeIncreaseRate;

    public EnemySpawnService(EnemySpawnServiceScriptableObject enemySpawnServiceScriptableObject)
    {
        startingNumberOfEnemies = enemySpawnServiceScriptableObject.StartingNumberOfEnemies;
        numberOfEnemiesInHorde = enemySpawnServiceScriptableObject.NumberOfEnemiesInHorde;
        currentNumberOfKillsToInitiateHorde = enemySpawnServiceScriptableObject.StartingNumberOfKillsToInitiateHorde;
        hordeIncreaseRate = enemySpawnServiceScriptableObject.HordeIncreaseRate;
        playerTransform = GameManager.Instance.PlayerController.transform;
        currentKillCountForHorde = 0;

        for (int i = 0; i < startingNumberOfEnemies; i++)
            SpawnEnemy();
        
        SubscribeToEvents();
    }
    
    ~EnemySpawnService()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnEnemyDied += OnEnemyDiedListener;
        GameManager.Instance.EventService.OnPlayerReachedMilestone += IncreaseSpawnRate;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnEnemyDied -= OnEnemyDiedListener;
        GameManager.Instance.EventService.OnPlayerReachedMilestone -= IncreaseSpawnRate;
    }

    private void IncreaseSpawnRate()
    {
        currentKillCountForHorde++;
        numberOfEnemiesInHorde += hordeIncreaseRate;
    }

    private void OnEnemyDiedListener(Vector3 enemyPosition)
    {
        if (currentKillCountForHorde <= currentNumberOfKillsToInitiateHorde)  
            SpawnEnemy();
        else
            SpawnHorde();
    }

    private void GetEnemyFromPool()
    {
        enemyToBeSpawned = GameManager.Instance.ObjectPoolingService.MainEnemyPool.GetEnemyFromPool();
        if (enemyToBeSpawned == null)
            return;
        enemyToBeSpawned.gameObject.SetActive(true);
    }

    //Using 2pi/enemynumber to get enemy position around a circle point in radians,
    //then multiplying it with random number to spawn on random points/equal points around the circle.
    private void SpawnEnemy()
    {
        GetEnemyFromPool();
        if(enemyToBeSpawned == null) return;
        
        float radiansPositionOnCircle = 2 * Mathf.PI / startingNumberOfEnemies * Random.Range(0f, startingNumberOfEnemies);
        enemyToBeSpawned.transform.position = GetCoordinatesOutsideOfPlayerView(radiansPositionOnCircle);
    }

    private void SpawnHorde()
    {
        currentKillCountForHorde = 0;
        for (int i = 0; i < numberOfEnemiesInHorde; i++)
        {
            GetEnemyFromPool();
            float radiansPositionOnCircle = 2 * Mathf.PI / numberOfEnemiesInHorde * i;
            if (enemyToBeSpawned == null) return;
            enemyToBeSpawned.transform.position = GetCoordinatesOutsideOfPlayerView(radiansPositionOnCircle);
        }
    }
    
    //Then convert the radian to a vector by using sin and cos since both gives us a point on the circle
    //And multiply it by some radius to get it out of player view
    private Vector3 GetCoordinatesOutsideOfPlayerView(float radians)
    {
        Vector3 pointAroundCircle = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * spawnCircleRadius;
        Vector2 enemySpawnPoint = playerTransform.position.normalized + pointAroundCircle;
        return enemySpawnPoint;
    }

    public void Pause()
    {
        UnsubscribeFromEvents();
    }

    public void Resume()
    {
        SubscribeToEvents();
    }
}
