using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolServiceScriptableObject", menuName = "ScriptableObject/ObjectPoolService")]
public class ObjectPoolServiceScriptableObject : ScriptableObject
{
    public EnemyController Merman_EnemyPrefab;
    public EnemyController Raven_EnemyPrefab;
    public int MaxEnemies;
    public GameObject XpPrefab;
    public int MaxXps;
}
