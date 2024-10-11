using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemySpawnServiceScriptableObject", menuName = "ScriptableObject/EnemySpawnService")]
public class EnemySpawnServiceScriptableObject : ScriptableObject
{
    public int StartingNumberOfEnemies;
    public int NumberOfEnemiesInHorde;
    public int StartingNumberOfKillsToInitiateHorde;
    public int HordeIncreaseRate;
}
