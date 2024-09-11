using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnServiceScriptableObject", menuName = "ScriptableObject/EnemySpawnService")]
public class EnemySpawnServiceScriptableObject : ScriptableObject
{
    public EnemyController EnemyPrefab;
    public int NumberOfEnemies;
}
