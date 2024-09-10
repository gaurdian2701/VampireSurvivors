using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    public int baseDamage { get; private set; }
    public float baseAttackSpeed { get; private set; }
    public float knockBackForce { get; private set; }
    public void InitWeaponData(WeaponScriptableObject weaponData)
    {
        baseDamage = weaponData.Damage;
        baseAttackSpeed = weaponData.AttackSpeed;
        this.knockBackForce = weaponData.KnockbackForce;
    }
    public virtual void Attack() { }
}
