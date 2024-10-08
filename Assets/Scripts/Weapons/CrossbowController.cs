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
        for (int i = 0; i < 4; i++)
        {
            ProjectileController projectileController =
                GameManager.Instance.ObjectPoolingService.ProjectilePool.GetProjectileFromPool();
            projectileController.gameObject.SetActive(true);
            projectileController.InitDamageData(BaseDamage, BaseKnockBackForce);
            Vector3 projectilePos = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
            currentAngle += (startingAngle - endingAngle) / 4f;
            projectileController.transform.position = transform.position + projectilePos;
            projectileController.transform.up = (projectileController.transform.position - transform.position).normalized;
        }
        WaitForCooldown();
    }

    private async void WaitForCooldown()
    {
        canShoot = false;
        await Task.Delay(milliseconds - (int)BaseAttackSpeed);
        canShoot = true;
    }
}
