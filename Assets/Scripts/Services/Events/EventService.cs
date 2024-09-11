using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    public delegate void OnEnemyDiedDelegate();

    public event OnEnemyDiedDelegate OnEnemyDied;
    public void InvokeEnemyDiedEvent() => OnEnemyDied?.Invoke();
}
