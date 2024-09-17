using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : StateMachine
{
    ~GameStateMachine()
    {
        UnsubscribeFromEvents();
    }
    public GameStateMachine(EnemySpawnService enemySpawnService, PlayerController playerController)
    {
        AddStates(enemySpawnService, playerController);
        SubscribeToEvents();
    }
    
    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPlayState += SwitchState <PlayingState>;
        GameManager.Instance.EventService.OnGameEnteredPauseState += SwitchState <PausedState>;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPlayState -= SwitchState <PlayingState>;
        GameManager.Instance.EventService.OnGameEnteredPauseState -= SwitchState <PausedState>;
    }

    protected override void AddStates(EnemySpawnService enemySpawnService, PlayerController playerController)
    {
        states = new List<State>();
        states.Add(new PlayingState(enemySpawnService, playerController));
        states.Add(new PausedState(enemySpawnService, playerController));
        Debug.Log("Added States");
    }
}
