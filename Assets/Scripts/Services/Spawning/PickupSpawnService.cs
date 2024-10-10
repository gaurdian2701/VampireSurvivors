using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawnService
{
    private GameObject xpPickupPrefab;
    public PickupSpawnService()
    {
        SubscribeToEvents();
    }

    ~PickupSpawnService()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnEnemyDied += SpawnPickup;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnEnemyDied -= SpawnPickup;
    }

    private void SpawnPickup(Vector3 spawnPosition)
    {
        xpPickupPrefab = GameManager.Instance.ObjectPoolingService.XpPool.GetXpFromPool();
        if(xpPickupPrefab != null)
            xpPickupPrefab.transform.position = spawnPosition;
    }
}
