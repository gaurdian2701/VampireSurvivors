using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    public delegate void OnEnemyDiedDelegate(EnemyClass enemyClass);
    public delegate void OnGameEnteredPlayStateDelegate();
    public delegate void OnGameEnteredPauseStateDelegate();
    public delegate void OnPlayerTookDamageDelegate(int damage);
    

    public event OnEnemyDiedDelegate OnEnemyDied;
    public event OnGameEnteredPlayStateDelegate OnGameEnteredPlayState;
    public event OnGameEnteredPauseStateDelegate OnGameEnteredPauseState;
    public event OnPlayerTookDamageDelegate OnPlayerTookDamage;
    public void InvokeEnemyDiedEvent(EnemyClass enemyClass) => OnEnemyDied?.Invoke(enemyClass);
    public void InvokeGameEnteredPlayStateEvent() => OnGameEnteredPlayState?.Invoke();
    public void InvokeGameEnteredPauseStateEvent() => OnGameEnteredPauseState?.Invoke();
    public void InvokePlayerTookDamageEvent(int damage) => OnPlayerTookDamage?.Invoke(damage);
}
