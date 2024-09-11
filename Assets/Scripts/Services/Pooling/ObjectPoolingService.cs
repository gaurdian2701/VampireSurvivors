using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingService
{
    public Merman_EnemyPool MermanEnemyPool;

    public ObjectPoolingService(ObjectPoolServiceScriptableObject objectPoolServiceData)
    {
        MermanEnemyPool = new Merman_EnemyPool(objectPoolServiceData.Merman_EnemyPrefab, objectPoolServiceData.MaxEnemies);
    }
}
