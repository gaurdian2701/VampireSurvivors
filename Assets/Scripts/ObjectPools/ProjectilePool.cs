using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : GenericObjectPool<ProjectileController>
{
    private ProjectileController projectileController;

    public ProjectilePool(ProjectileController projectileController, int maxObjectsInPool)
    {
        this.projectileController = projectileController;
        this.maxObjectsInPool = maxObjectsInPool;
    }

    public ProjectileController GetProjectileFromPool()
    {
        return GetObjectFromPool();
    }

    protected override ProjectileController CreateNewObject()
    {
        return GameObject.Instantiate(projectileController);
    }
}
