using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObject/EnemyScriptableObject")]
public class EnemyScriptableObject : ScriptableObject
{
    [FormerlySerializedAs("MaxHealth")] public int EnmeyMaxHealth;
    [FormerlySerializedAs("Damage")] public int EnemyDamage;
    public float EnemySpeed;
    public float EnemySpeedModifier;
    [FormerlySerializedAs("StoppingDistance")] public float EnemyStoppingDistance;
    public Sprite EnemyImage;
    public EnemyMovementType EnemyMovementType;
    public EnemyClass EnemyClass;
}
