using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merman_EnemyPool : GenericObjectPool<EnemyController>
{
    private EnemyController enemyPrefab;

    public Merman_EnemyPool(EnemyController enemyPrefab, int maxObjectsInPool)
    {
        this.enemyPrefab = enemyPrefab;
        this.maxObjectsInPool = maxObjectsInPool;
    }

    public EnemyController GetEnemyFromPool()
    {
        return GetObjectFromPool();
    }
    protected override EnemyController CreateNewObject()
    {
        EnemyController enemy = GameObject.Instantiate(enemyPrefab);
        return enemy;
    }
}
