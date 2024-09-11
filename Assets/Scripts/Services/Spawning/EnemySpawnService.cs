using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnService
{
    private Camera mainCamera;
    private EnemyController enemyToBeSpawned;
    private Transform playerTransform;
    private int startingNumberOfEnemies;
    
    private static float spawnCircleRadius = 7f;

    public EnemySpawnService(EnemySpawnServiceScriptableObject enemySpawnControllerScriptableObject, Camera mainCamera)
    {
        this.mainCamera = mainCamera;
        startingNumberOfEnemies = enemySpawnControllerScriptableObject.StartingNumberOfEnemies;
        playerTransform = GameManager.Instance.Player.transform;

        for (int i = 0; i < startingNumberOfEnemies; i++)
            SpawnEnemies();

        ListenToEvents();
        SpawnEnemies();
    }

    private void ListenToEvents()
    {
        GameManager.Instance.EventService.OnEnemyDied += SpawnEnemies;
    }

    private void SpawnEnemies()
    {
        enemyToBeSpawned = GameManager.Instance.ObjectPoolingService.MermanEnemyPool.GetObjectFromPool();
        if (enemyToBeSpawned == null)
            return;
        enemyToBeSpawned.gameObject.SetActive(true);
        enemyToBeSpawned.transform.position = GetCoordinatesOutsideOfPlayerView();
    }

    private Vector3 GetCoordinatesOutsideOfPlayerView()
    {
        //Using 2pi/enemynumber to get enemy position around a circle point in radians, then multiplying it with enemyPos to spawn it at different points on the circle.
        float radians = 2 * Mathf.PI / startingNumberOfEnemies * Random.Range(0f, startingNumberOfEnemies);

        //Then convert the radian to a vector by using sin and cos since both gives us a point on the circle
        Vector3 pointAroundCircle = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * spawnCircleRadius;

        //Then multiply it by some radius to get it out of player view
        Vector2 enemySpawnPoint = playerTransform.position.normalized + pointAroundCircle;
        return enemySpawnPoint;
    }
}
