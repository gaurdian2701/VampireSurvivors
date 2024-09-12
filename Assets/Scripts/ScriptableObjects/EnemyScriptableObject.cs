using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObject/EnemyScriptableObject")]
public class EnemyScriptableObject : ScriptableObject
{
    public int MaxHealth;
    public float EnemySpeed;
    public float EnemySpeedModifier;
    public float StoppingDistance;
    public Sprite EnemyImage;
}
