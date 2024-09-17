using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    public delegate void OnEnemyDiedDelegate(EnemyClass enemyClass);
    public delegate void OnGameEnteredPlayStateDelegate();
    public delegate void OnGameEnteredPauseStateDelegate();
    

    public event OnEnemyDiedDelegate OnEnemyDied;
    public event OnGameEnteredPlayStateDelegate OnGameEnteredPlayState;
    public event OnGameEnteredPauseStateDelegate OnGameEnteredPauseState;
    public void InvokeEnemyDiedEvent(EnemyClass enemyClass) => OnEnemyDied?.Invoke(enemyClass);
    public void InvokeGameEnteredPlayStateEvent() => OnGameEnteredPlayState?.Invoke();
    public void InvokeGameEnteredPauseStateEvent() => OnGameEnteredPauseState?.Invoke();
}
