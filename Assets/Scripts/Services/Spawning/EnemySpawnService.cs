using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnService : IPausable
{
    private EnemyController enemyToBeSpawned;
    private Transform playerTransform;
    private int startingNumberOfEnemies;
    private int numberOfEnemiesInHorde;
    
    private static float spawnCircleRadius = 15f;
    private static int currentNumberOfKillsToInitiateHorde;
    private int currentKillCountForHorde;
    private int numberOEnemyTypes = Enum.GetNames(typeof(EnemyClass)).Length;

    public EnemySpawnService(EnemySpawnServiceScriptableObject enemySpawnServiceScriptableObject)
    {
        startingNumberOfEnemies = enemySpawnServiceScriptableObject.StartingNumberOfEnemies;
        numberOfEnemiesInHorde = enemySpawnServiceScriptableObject.NumberOfEnemiesInHorde;
        currentNumberOfKillsToInitiateHorde = enemySpawnServiceScriptableObject.StartingNumberOfKillsToInitiateHorde;
        playerTransform = GameManager.Instance.PlayerController.transform;
        currentKillCountForHorde = 0;

        for (int i = 0; i < startingNumberOfEnemies; i++)
        {
            SpawnEnemy(EnemyClass.MERMAN);
            SpawnEnemy(EnemyClass.RAVEN);
        }
        SubscribeToEvents();
    }
    
    ~EnemySpawnService()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnEnemyDied += OnEnemyDiedListener;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnEnemyDied -= OnEnemyDiedListener;
    }

    private void OnEnemyDiedListener(Vector3 enemyPosition) //CHANGE THIS TO SPAWN RANDOM ENEMIES INSTEAD OF HAVING TO GET ENEMY CLASS FROM EVENT LISTENER
    {
        int classToSpawn = Random.Range(0, numberOEnemyTypes);
        currentKillCountForHorde++;
        if (currentKillCountForHorde <= currentNumberOfKillsToInitiateHorde)  
            SpawnEnemy((EnemyClass)classToSpawn);
        else
            SpawnHorde((EnemyClass)classToSpawn);
    }

    private void GetEnemyFromPool(EnemyClass enemyClass)
    {
        switch (enemyClass)
        {
            case EnemyClass.MERMAN :
                enemyToBeSpawned = GameManager.Instance.ObjectPoolingService.Merman_EnemyPool.GetEnemyFromPool();
                break;
            
            case EnemyClass.RAVEN:
                enemyToBeSpawned = GameManager.Instance.ObjectPoolingService.Raven_EnemyPool.GetEnemyFromPool();
                break;
        }
        if (enemyToBeSpawned == null)
            return;
        enemyToBeSpawned.gameObject.SetActive(true);
    }

    private void SpawnEnemy(EnemyClass enemyClass)
    {
        GetEnemyFromPool(enemyClass);
        if(enemyToBeSpawned == null) return;
        //Using 2pi/enemynumber to get enemy position around a circle point in radians,
        //then multiplying it with random number to spawn on random points/equal points around the circle.
        float radiansPositionOnCircle = 2 * Mathf.PI / startingNumberOfEnemies * Random.Range(0f, startingNumberOfEnemies);
        enemyToBeSpawned.transform.position = GetCoordinatesOutsideOfPlayerView(radiansPositionOnCircle);
    }
    private void SpawnHorde(EnemyClass enemyClass)
    {
        currentKillCountForHorde = 0;
        currentNumberOfKillsToInitiateHorde += 5;
        for (int i = 0; i < numberOfEnemiesInHorde; i++)
        {
            GetEnemyFromPool(enemyClass);
            float radiansPositionOnCircle = 2 * Mathf.PI / numberOfEnemiesInHorde * i;
            if (enemyToBeSpawned == null) return;
            enemyToBeSpawned.transform.position = GetCoordinatesOutsideOfPlayerView(radiansPositionOnCircle);
        }
    }
    private Vector3 GetCoordinatesOutsideOfPlayerView(float radians)
    {
        //Then convert the radian to a vector by using sin and cos since both gives us a point on the circle
        Vector3 pointAroundCircle = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * spawnCircleRadius;

        //Then multiply it by some radius to get it out of player view
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
