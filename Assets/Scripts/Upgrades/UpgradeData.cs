using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class UpgradeData
{
    public string UpgradeName;
    public WeaponType WeaponType;
    public int Damage; 
    public float AttackSpeed; 
    public int Knockback;
    public int NumberOfWeapons;
    public float ProjectileSpread;

    public void InitializeBaseUpgradeStats(string UpgradeName,
        WeaponType WeaponType,
        int Damage,
        float AttackSpeed,
        int Knockback,
        int NumberOfWeapons,
        float ProjectileSpread)
    {
        this.UpgradeName = UpgradeName;
        this.WeaponType = WeaponType;
        this.Damage = Damage;
        this.AttackSpeed = AttackSpeed;
        this.Knockback = Knockback;
        this.NumberOfWeapons = NumberOfWeapons;
        this.ProjectileSpread = ProjectileSpread;
    }
}