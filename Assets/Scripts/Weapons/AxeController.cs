using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : Weapon
{
    [SerializeField] private WeaponScriptableObject weaponData;

    private void Awake()
    {
        InitWeaponData(weaponData);
        SubscribeToEvents();
    }
    
    public override void Attack()
    {
        transform.RotateAround(playerTransform.position, Vector3.forward, BaseAttackSpeed * Time.deltaTime);
    }
}
