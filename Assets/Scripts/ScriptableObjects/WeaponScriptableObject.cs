using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObject/NewWeapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public int Damage;
    public float AttackSpeed;
    public float KnockbackForce;
    public Sprite WeaponImage;
    public Vector3 LocalPosition;
    public Vector3 LocalRotation;
    public Vector3 Scale;
}
