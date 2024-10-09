using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CrossbowController : Weapon
{
    [SerializeField] private WeaponScriptableObject weaponData;

    private bool canShoot = true;
    private const int milliseconds = 1000;
    private float degreeOffset = 30f;
    private int arrowsLossenedPerShot = 1;
    private void Awake()
    {
        InitWeaponData(weaponData);
        SubscribeToEvents();
    }
    
    public override void Attack()
    {
        if (!canShoot)
            return;

        float offsetInRadians = Mathf.Deg2Rad * degreeOffset;
        float middleAngle = Mathf.Atan2(transform.up.y, transform.up.x);
        float startingAngle = middleAngle + offsetInRadians / 2;
        float endingAngle = middleAngle - offsetInRadians / 2;
        float currentAngle = startingAngle;

        if (arrowsLossenedPerShot == 1)
            currentAngle = middleAngle;
        
        for (int i = 0; i < arrowsLossenedPerShot; i++)
        {
            ProjectileController projectileController = PrepareArrow();
            Vector3 projectilePos = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
            currentAngle += (startingAngle - endingAngle) / arrowsLossenedPerShot;
            projectileController.transform.position += projectilePos;
            projectileController.transform.up = (projectileController.transform.position - transform.position).normalized;
        }
        WaitForCooldown();
    }

    private ProjectileController PrepareArrow()
    {
        ProjectileController projectileController =
            GameManager.Instance.ObjectPoolingService.ProjectilePool.GetProjectileFromPool();
        projectileController.gameObject.SetActive(true);
        projectileController.InitDamageData(BaseDamage, BaseKnockBackForce);
        projectileController.transform.position = transform.position;
        return projectileController;
    }

    private async void WaitForCooldown()
    {
        canShoot = false;
        await Task.Delay(milliseconds - (int)BaseAttackSpeed);
        canShoot = true;
    }
}
