using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnService
{
    private Camera mainCamera;
    private EnemyController enemyToBeSpawned;
    private Transform playerTransform;
    private int startingNumberOfEnemies;
    private int numberOfEnemiesInHorde;
    
    private static float spawnCircleRadius = 12f;
    private static int currentNumberOfKillsToInitiateHorde;
    private int currentKillCountForHorde;

    public EnemySpawnService(EnemySpawnServiceScriptableObject enemySpawnServiceScriptableObject, Camera mainCamera)
    {
        this.mainCamera = mainCamera;
        startingNumberOfEnemies = enemySpawnServiceScriptableObject.StartingNumberOfEnemies;
        numberOfEnemiesInHorde = enemySpawnServiceScriptableObject.NumberOfEnemiesInHorde;
        currentNumberOfKillsToInitiateHorde = enemySpawnServiceScriptableObject.StartingNumberOfKillsToInitiateHorde;
        playerTransform = GameManager.Instance.Player.transform;
        currentKillCountForHorde = 0;

        for (int i = 0; i < startingNumberOfEnemies; i++)   
            SpawnEnemy();

        ListenToEvents();
    }

    private void ListenToEvents()
    {
        GameManager.Instance.EventService.OnEnemyDied += OnEnemyDiedListener;
    }

    private void OnEnemyDiedListener()
    {
        currentKillCountForHorde++;
        if (currentKillCountForHorde <= currentNumberOfKillsToInitiateHorde)  
            SpawnEnemy();
        else
            SpawnHorde();
    }

    private void GetEnemyFromPool()
    {
        enemyToBeSpawned = GameManager.Instance.ObjectPoolingService.MermanEnemyPool.GetObjectFromPool();
        if (enemyToBeSpawned == null)
            return;
        enemyToBeSpawned.gameObject.SetActive(true);
    }

    private void SpawnEnemy()
    {
        GetEnemyFromPool();
        if(enemyToBeSpawned == null) return;
        //Using 2pi/enemynumber to get enemy position around a circle point in radians,
        //then multiplying it with random number to spawn on random points/equal points around the circle.
        float radiansPositionOnCircle = 2 * Mathf.PI / startingNumberOfEnemies * Random.Range(0f, startingNumberOfEnemies);
        enemyToBeSpawned.transform.position = GetCoordinatesOutsideOfPlayerView(radiansPositionOnCircle);
    }
    private void SpawnHorde()
    {
        currentKillCountForHorde = 0;
        currentNumberOfKillsToInitiateHorde = 5;
        for (int i = 0; i < numberOfEnemiesInHorde; i++)
        {
            GetEnemyFromPool();
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
}
