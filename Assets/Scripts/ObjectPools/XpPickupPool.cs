using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpPickupPool : GenericObjectPool<GameObject>
{
    private GameObject xpPickupPrefab;

    public XpPickupPool(GameObject xpPickupPrefab, int maxObjectsInPool)
    {
        this.xpPickupPrefab = xpPickupPrefab;
        this.maxObjectsInPool = maxObjectsInPool;
    }

    public GameObject GetXpFromPool() 
    {
        GameObject xpToReturn = GetObjectFromPool();
        xpToReturn?.SetActive(true);
        return xpToReturn;
    }
    protected override GameObject CreateNewObject()
    {
        GameObject xpPickup = GameObject.Instantiate(xpPickupPrefab);
        return xpPickup;
    } 
}
