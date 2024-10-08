using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CrossbowController : Weapon
{
    [SerializeField] private WeaponScriptableObject weaponData;

    private bool canShoot = true;
    private int milliseconds = 1000;
    private float positionalOffset = 0.25f;
    private void Awake()
    {
        InitWeaponData(weaponData);
        SubscribeToEvents();
    }

    //DO THE THING YOU REMEMBERED THAT WORKED AND IT WAS COOL
    public override void Attack()
    {
        if (!canShoot)
            return;
        float angle = Mathf.Atan2(transform.up.y, transform.up.x);
        for (int i = 0; i < 2; i++)
        {
            ProjectileController projectileController =
                GameManager.Instance.ObjectPoolingService.ProjectilePool.GetProjectileFromPool();
            projectileController.gameObject.SetActive(true);
            projectileController.InitDamageData(BaseDamage, BaseKnockBackForce);
            float currentAngle = angle + positionalOffset;
            positionalOffset *= -1f;
            Vector3 projectilePos = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
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
