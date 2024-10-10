using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    public WeaponType WeaponType;
    public int BaseDamage { get; private set; }
    public int NumberOfWeaponsToSpawn { get; private set; }
    public float BaseAttackSpeed { get; private set; }
    public float BaseKnockBackForce { get; private set; }
    
    protected Transform playerTransform;
    
    public void InitWeaponData(WeaponScriptableObject weaponData)
    {
        BaseDamage = weaponData.Damage;
        BaseAttackSpeed = weaponData.AttackSpeed;
        BaseKnockBackForce = weaponData.KnockbackForce;
        SubscribeToEvents();
    }

    private void OnDestroy() => UnsubscribeFromEvents();

    protected void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnPlayerPressedAttackButton += Attack;
    }

    protected void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnPlayerPressedAttackButton -= Attack;
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

    public void InitalizeBaseStats(int Damage,
        float AttackSpeed,
        float KnockbackForce)
    {
        BaseDamage = Damage;
        BaseAttackSpeed = AttackSpeed;
        BaseKnockBackForce = KnockbackForce;
    }
    
    public void UpgradeBaseStats(UpgradeData upgradeData)
    {
        BaseDamage += upgradeData.Damage;
        BaseAttackSpeed += upgradeData.AttackSpeed;
        BaseKnockBackForce += upgradeData.Knockback;
    }
}
