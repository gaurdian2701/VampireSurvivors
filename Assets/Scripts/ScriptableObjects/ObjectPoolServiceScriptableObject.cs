using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ObjectPoolServiceScriptableObject", menuName = "ScriptableObject/ObjectPoolService")]
public class ObjectPoolServiceScriptableObject : ScriptableObject
{
    [FormerlySerializedAs("EnemyPrefabs")] public List<EnemyController> EnemyPrefabsList;
    public int MaxEnemies;
    public GameObject XpPrefab;
    public int MaxXps;
}
