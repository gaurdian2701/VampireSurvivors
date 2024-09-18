using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public List<State> states;
    protected State currentState;
    protected virtual void AddStates(EnemySpawnService enemySpawnService, PlayerController playerController){}

    public void UpdateStateMachine()
    {
        currentState?.UpdateState();
    }

    protected void SwitchState<TanyState>()
    {
        foreach (State state in states)
        {
            if (state.GetType() == typeof(TanyState))
            {
                currentState?.ExitState();
                currentState = state;
                currentState?.EnterState();
                return;
            }
        }
        Debug.LogError("State does not match type");
    }
}
