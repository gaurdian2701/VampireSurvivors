using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ObjectPoolingServiceScriptableObject", menuName = "ScriptableObject/ObjectPoolingService")]
public class ObjectPoolingServiceScriptableObject : ScriptableObject
{
    [FormerlySerializedAs("EnemyPrefabs")] public List<EnemyController> EnemyPrefabsList;
    public int MaxEnemies;
    public GameObject XpPrefab;
    public int MaxXps;
    public ProjectileController ProjectilePrefab;
    public int MaxProjectiles;
}
