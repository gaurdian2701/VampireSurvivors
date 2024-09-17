using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedState : State
{
    private EnemySpawnService enemySpawnService;
    private PlayerController playerController;

    public PausedState(EnemySpawnService enemySpawnService,
        PlayerController playerController)
    {
        this.enemySpawnService = enemySpawnService;
        this.playerController = playerController;
    }
    public override void EnterState()
    {
        enemySpawnService.Pause();
        playerController.Pause();
    }

    public override void UpdateState()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.EventService.InvokeGameEnteredPlayStateEvent();
    }

    public override void ExitState()
    {
    }
}
