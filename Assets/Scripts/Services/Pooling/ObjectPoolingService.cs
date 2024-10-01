using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingService
{
    public EnemyPool MainEnemyPool;
    public XpPickupPool XpPool;

    public ObjectPoolingService(ObjectPoolServiceScriptableObject objectPoolServiceData)
    {
        MainEnemyPool = new EnemyPool(objectPoolServiceData.EnemyPrefabsList, objectPoolServiceData.MaxEnemies);
        XpPool = new XpPickupPool(objectPoolServiceData.XpPrefab, objectPoolServiceData.MaxXps); 
    }
}
