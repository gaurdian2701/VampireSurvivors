using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemyPool : GenericObjectPool<EnemyController>
{
    private List<EnemyController> enemyPrefabsList;
    
    public EnemyPool(List<EnemyController> enemyPrefabsList, int maxObjectsInPool)
    {
        this.enemyPrefabsList = enemyPrefabsList;
        this.maxObjectsInPool = maxObjectsInPool;
    }

    public EnemyController GetEnemyFromPool() 
    {
        return GetObjectFromPool();
    }
    
    protected override EnemyController CreateNewObject()
    {
        Random random = new Random();
        return GameObject.Instantiate(enemyPrefabsList[random.Next(0, enemyPrefabsList.Count)]);
    } 
}
