using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    public int BaseDamage { get; private set; }
    public int NumberOfWeaponsToSpawn { get; private set; }
    public float BaseAttackSpeed { get; private set; }
    public float BaseKnockBackForce { get; private set; }
    
    protected Transform playerTransform;
    
    public readonly int baseDamageIncreaseRate = 2;
    public readonly float baseAttackSpeedIncreaseRate = 50f;
    public readonly float baseKnockBackForceIncreaseRate = 1f;
    public void InitWeaponData(WeaponScriptableObject weaponData)
    {
        BaseDamage = weaponData.Damage;
        BaseAttackSpeed = weaponData.AttackSpeed;
        BaseKnockBackForce = weaponData.KnockbackForce;
    }

    public void InitializeWeaponPositionAndOrientation(Transform playerTransform, 
        Vector3 weaponLocalPositionOffset)
    {
        transform.SetParent(playerTransform);
        this.playerTransform = playerTransform;
        transform.localPosition = weaponLocalPositionOffset;
        Vector3 vectorAwayFromPlayerBody = transform.position - playerTransform.position;
        transform.up = vectorAwayFromPlayerBody.normalized;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyController enemy))
            enemy.TakeDamage(BaseDamage, BaseKnockBackForce);
    }
    public virtual void Attack() { }
    public void SetBaseDamage(int someDamage) => BaseDamage = someDamage;
    public void SetBaseAttackSpeed(float someAttackSpeed) => BaseAttackSpeed = someAttackSpeed;
    public void SetBaseKnockBackForce(float someKnockBackForce) => BaseKnockBackForce = someKnockBackForce;
}
