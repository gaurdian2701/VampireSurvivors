using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : State
{
    private EnemySpawnService enemySpawnService;
    private PlayerController playerController;

    public PlayingState(EnemySpawnService enemySpawnService,
        PlayerController playerController)
    {
        this.enemySpawnService = enemySpawnService;
        this.playerController = playerController;
    }
    public override void EnterState()
    {
        enemySpawnService.Resume();
        playerController.Resume();
    }

    public override void UpdateState()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.EventService.InvokeGameEnteredPauseStateEvent();
    }

    public override void ExitState()
    {
    }
}
