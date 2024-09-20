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
    
    protected Transform playerBodyTransform;
    
    private static int baseDamageIncreaseRate = 2;
    private static float baseAttackSpeedIncreaseRate = 50f;
    public void InitWeaponData(WeaponScriptableObject weaponData)
    {
        BaseDamage = weaponData.Damage;
        BaseAttackSpeed = weaponData.AttackSpeed;
        BaseKnockBackForce = weaponData.KnockbackForce;
    }
    
    public void InitializeWeaponPositionAndOrientation(Transform playerTransform, Transform playerBodyTransform,
        Vector3 weaponLocalPositionOffset)
    {
        transform.SetParent(playerTransform);
        this.playerBodyTransform = playerBodyTransform;
        transform.localPosition = playerBodyTransform.localPosition + weaponLocalPositionOffset;
        Vector3 vectorAwayFromPlayerBody = transform.position - playerBodyTransform.position;
        transform.up = vectorAwayFromPlayerBody.normalized;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyController enemy))
            enemy.TakeDamage(BaseDamage, BaseKnockBackForce);
    }
    public virtual void Attack() { }
    
    public void IncreaseNumberOfWeaponsToSpawn() => NumberOfWeaponsToSpawn++;
    public void IncreaseBaseDamage() => BaseDamage += baseDamageIncreaseRate;
    public void IncreaseBaseAttackSpeed() => BaseAttackSpeed += baseAttackSpeedIncreaseRate;
    public void IncreaseBaseKnockBackForce() => BaseKnockBackForce++;
}
