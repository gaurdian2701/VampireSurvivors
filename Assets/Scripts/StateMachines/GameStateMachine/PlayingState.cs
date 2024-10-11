using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : State
{
    private EnemySpawnService enemySpawnService;
    private PlayerController playerController;
    private GameManager gameManager;
    private bool isPausedOnPlayerLevelUp = false;
    
    public PlayingState(EnemySpawnService enemySpawnService,
        PlayerController playerController,
        GameManager gameManager)
    {
        this.enemySpawnService = enemySpawnService;
        this.playerController = playerController;
        this.gameManager = gameManager;
        SubscribeToEvents();
    }

    ~PlayingState()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnPlayerLevelledUp += PauseGameOnLevelUp;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnPlayerLevelledUp -= PauseGameOnLevelUp;
    }

    public override void EnterState()
    {
        enemySpawnService.Resume();
        playerController.Resume();
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.CanPause)
        {
            gameManager.ChangeGamePauseType(GamePauseType.PauseOnPlayerInput);
            gameManager.EventService.InvokeGameEnteredPauseStateEvent();
        }
    }

    public override void ExitState()
    {
    }

    private void PauseGameOnLevelUp()
    {
        isPausedOnPlayerLevelUp = true;
        gameManager.ChangeGamePauseType(GamePauseType.PauseOnPlayerLevelUp);
        gameManager.EventService.InvokeGameEnteredPauseStateEvent();
    }
}
