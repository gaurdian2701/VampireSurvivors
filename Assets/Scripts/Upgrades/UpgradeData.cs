using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class UpgradeData
{
    [FormerlySerializedAs("UpgradeDamage")] public int Damage;
    [FormerlySerializedAs("UpgradeAttackSpeed")] public int AttackSpeed;
    [FormerlySerializedAs("UpgradeKnockback")] public int Knockback;
    [FormerlySerializedAs("UpgradeNumberOfWeapons")] public int NumberOfWeapons;
    public float AttackSpread;
}