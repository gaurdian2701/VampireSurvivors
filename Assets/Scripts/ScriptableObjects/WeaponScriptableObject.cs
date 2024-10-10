using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObject/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public WeaponType WeaponType;
    public int Damage;
    public float AttackSpeed;
    public float KnockbackForce;
}
