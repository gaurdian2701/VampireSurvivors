using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    public delegate void OnEnemyDiedDelegate(Vector3 enemyDeathLocation); 
    public delegate void OnGameEnteredPlayStateDelegate();
    public delegate void OnGameEnteredPauseStateDelegate();
    public delegate void OnPlayerTookDamageDelegate(int damage);
    public delegate void OnPlayerPickedUpXpDelegate();
    public delegate void OnPlayerLevelledUpDelegate();
    public delegate void OnPlayerPressedAttackButtonDelegate();
    public delegate void OnPlayerSelectedUpgradeEventDelegate();
    public delegate void OnPlayerDiedDelegate();
    public delegate void OnPlayerReachedMilestoneDelegate();
    
    
    public event OnEnemyDiedDelegate OnEnemyDied;
    public event OnGameEnteredPlayStateDelegate OnGameEnteredPlayState;
    public event OnGameEnteredPauseStateDelegate OnGameEnteredPauseState;
    public event OnPlayerTookDamageDelegate OnPlayerTookDamage;
    public event OnPlayerPickedUpXpDelegate OnPlayerPickedUpXp;
    public event OnPlayerLevelledUpDelegate OnPlayerLevelledUp;
    public event OnPlayerPressedAttackButtonDelegate OnPlayerPressedAttackButton;
    public event OnPlayerSelectedUpgradeEventDelegate OnPlayerSelectedUpgrade;
    public event OnPlayerDiedDelegate OnPlayerDied;
    public event OnPlayerReachedMilestoneDelegate OnPlayerReachedMilestone;
    
    
    public void InvokeEnemyDiedEvent(Vector3 enemyDeathLocation) => OnEnemyDied?.Invoke(enemyDeathLocation);
    public void InvokeGameEnteredPlayStateEvent() => OnGameEnteredPlayState?.Invoke();
    public void InvokeGameEnteredPauseStateEvent() => OnGameEnteredPauseState?.Invoke();
    public void InvokePlayerTookDamageEvent(int damage) => OnPlayerTookDamage?.Invoke(damage);
    public void InvokePlayerPickedUpXpEvent() => OnPlayerPickedUpXp?.Invoke();
    public void InvokePLayerLevelledUpEvent() => OnPlayerLevelledUp?.Invoke();
    public void InvokePlayerPressedAttackButtonEvent() => OnPlayerPressedAttackButton?.Invoke();
    public void InvokePlayerSelectedUpgradeEvent() => OnPlayerSelectedUpgrade?.Invoke();
    public void InvokePlayerDiedEvent() => OnPlayerDied?.Invoke();
    public void InvokePlayerReachedMilestoneEvent() => OnPlayerReachedMilestone?.Invoke();
}
