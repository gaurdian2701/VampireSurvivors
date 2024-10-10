using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObject/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public Sprite EnemySprite;
    [FormerlySerializedAs("EnmeyMaxHealth")] [FormerlySerializedAs("MaxHealth")] public int EnemyMaxHealth;
    [FormerlySerializedAs("Damage")] public int EnemyDamage;
    public float EnemySpeed;
    [FormerlySerializedAs("StoppingDistance")] public float EnemyStoppingDistance;
    public EnemyMovementType EnemyMovementType;
    [FormerlySerializedAs("MaxHealthStatIncreaseRate")] public int EnemyMaxHealthStatIncreaseRate;
    [FormerlySerializedAs("SpeedStatIncreaseRate")] public float EnemySpeedStatIncreaseRate;
    public int DamageStatIncreaseRate;
}
