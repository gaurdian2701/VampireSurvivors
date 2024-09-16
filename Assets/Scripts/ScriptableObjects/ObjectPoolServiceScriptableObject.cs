using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolServiceScriptableObject", menuName = "ScriptableObject/ObjectPoolServiceScriptableObject")]
public class ObjectPoolServiceScriptableObject : ScriptableObject
{
    public EnemyController Merman_EnemyPrefab;
    public EnemyController Raven_EnemyPrefab;
    public int MaxEnemies;
}
