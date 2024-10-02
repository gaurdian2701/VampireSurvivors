using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingService
{
    public EnemyPool MainEnemyPool;
    public XpPickupPool XpPool;

    public ObjectPoolingService(ObjectPoolingServiceScriptableObject objectPoolingServiceData)
    {
        MainEnemyPool = new EnemyPool(objectPoolingServiceData.EnemyPrefabsList, objectPoolingServiceData.MaxEnemies);
        XpPool = new XpPickupPool(objectPoolingServiceData.XpPrefab, objectPoolingServiceData.MaxXps); 
    }
}
