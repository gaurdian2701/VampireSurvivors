using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class UpgradeData
{
    [FormerlySerializedAs("UpgradeDamage")] public int Damage;
    [FormerlySerializedAs("UpgradeAttackSpeed")] public float AttackSpeed;
    [FormerlySerializedAs("UpgradeKnockback")] public int Knockback;

    public void InitializeBaseUpgradeStats(int Damage,
        float AttackSpeed,
        int Knockback)
    {
        this.Damage = Damage;
        this.AttackSpeed = AttackSpeed;
        this.Knockback = Knockback;
    }
}