using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedState : State
{
    private EnemySpawnService enemySpawnService;
    private PlayerController playerController;
    private GameManager gameManager;
    private bool pausedOnUpgrade;

    public PausedState(EnemySpawnService enemySpawnService,
        PlayerController playerController,
        GameManager gameManager)
    {
        this.enemySpawnService = enemySpawnService;
        this.playerController = playerController;
        this.gameManager = gameManager;
    }
    public override void EnterState()
    {
        enemySpawnService.Pause();
        playerController.Pause();
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.currentGamePauseType != GamePauseType.PauseOnPlayerLevelUp)
        {
            gameManager.ChangeGamePauseType(GamePauseType.PauseOnPlayerInput);
            gameManager.EventService.InvokeGameEnteredPlayStateEvent();
        }
    }

    public override void ExitState()
    {
    }
}
