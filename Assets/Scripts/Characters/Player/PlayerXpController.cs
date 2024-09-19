using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerXpController
{
    public int currentXpToNextLevel { get; private set; }
    private int currentXp = 0;
    private int maxLevel;

    public PlayerXpController(PlayerXpControllerScriptableObject playerXpControllerScriptableObject)
    {
        currentXpToNextLevel = playerXpControllerScriptableObject.StartingPlayerXpToNextLevel;
        maxLevel = playerXpControllerScriptableObject.PlayerMaxLevel;
        SubscribeToEvents();
    }

    ~PlayerXpController()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnPlayerPickedUpXp += IncreaseXp;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnPlayerPickedUpXp -= IncreaseXp;
    }

    private void IncreaseXp()
    {
        currentXp++;
        if (currentXp >= currentXpToNextLevel)
        {
            GameManager.Instance.EventService.InvokePLayerLevelledUpEvent();
            currentXp = 0;
        }
    }
}
