using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingService
{
    public EnemyPool Merman_EnemyPool;
    public EnemyPool Raven_EnemyPool;
    public XpPickupPool XpPool;

    public ObjectPoolingService(ObjectPoolServiceScriptableObject objectPoolServiceData)
    {
        Merman_EnemyPool = new EnemyPool(objectPoolServiceData.Merman_EnemyPrefab, objectPoolServiceData.MaxEnemies);
        Raven_EnemyPool = new EnemyPool(objectPoolServiceData.Raven_EnemyPrefab, objectPoolServiceData.MaxEnemies);
        XpPool = new XpPickupPool(objectPoolServiceData.XpPrefab, objectPoolServiceData.MaxXps);
    }
}
