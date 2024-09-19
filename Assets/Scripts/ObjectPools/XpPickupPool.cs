using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpPickupPool : GenericObjectPool<GameObject>
{
    private GameObject xpPickupPrefab;

    public XpPickupPool(GameObject xpPrefab, int maxObjectsInPool)
    {
        this.xpPickupPrefab = xpPrefab;
        this.maxObjectsInPool = maxObjectsInPool;
    }

    public GameObject GetXpFromPool() 
    {
        return GetObjectFromPool();
    }
    protected override GameObject CreateNewObject()
    {
        GameObject xpPickup = GameObject.Instantiate(xpPickupPrefab);
        return xpPickup;
    } 
}
