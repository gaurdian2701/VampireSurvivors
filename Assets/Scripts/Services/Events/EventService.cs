using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    public delegate void OnEnemyDiedDelegate(EnemyClass enemyClass);

    public event OnEnemyDiedDelegate OnEnemyDied;
    public void InvokeEnemyDiedEvent(EnemyClass enemyClass) => OnEnemyDied?.Invoke(enemyClass);
}
