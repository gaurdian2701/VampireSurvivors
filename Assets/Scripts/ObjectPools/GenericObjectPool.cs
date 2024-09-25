using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> where T : class 
{
    protected int maxObjectsInPool;
    protected List<PooledObject<T>> pooledObjects = new List<PooledObject<T>>();

    protected virtual T GetObjectFromPool()
    {
        if (pooledObjects.Count > 0)
        {
            PooledObject<T> pooledObject = pooledObjects.Find(x => x.IsUsed == false);
            if (pooledObject != null)
            {
                pooledObject.IsUsed = true;
                return pooledObject.Object;
            }
        }
        
        // if(pooledObjects.Count != 0)
        //     Debug.Log("Failed tp find object in the pool for " + pooledObjects[0].Object);
        return CreateNewPooledObject();
    }
    
    private T CreateNewPooledObject()
    {
        if (pooledObjects.Count == maxObjectsInPool)
            return null;

        PooledObject<T> pooledObject = new PooledObject<T>();
        pooledObject.Object = CreateNewObject();
        pooledObject.IsUsed = true;
        pooledObjects.Add(pooledObject);
        return pooledObject.Object;
    }

    protected virtual T CreateNewObject()
    {
        throw new System.NotImplementedException();
    }

    public void ReturnObjectToPool(T obj)
    {
        PooledObject<T> pooledObject = pooledObjects.Find(x => x.Object.Equals(obj));
        if (pooledObject != null)
        {
            pooledObject.IsUsed = false;
            Debug.Log("Found object in Pool for " + obj);
        }
        else 
            Debug.Log("No object in Pool for " + obj);
    }
    public class PooledObject<T>
    {
        public T Object;
        public bool IsUsed;
    }
}
